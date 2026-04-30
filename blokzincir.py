#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
blokzincir.py
-------------
Blokzincir ağının ana mantık katmanı.

İçerdiği sınıflar
-----------------
BlockchainNetwork    : NetworkX DiGraph + hash tablosu üzerine kurulu ağ yöneticisi
FundFlowAnalyzer     : BFS (Queue) ve DFS (Stack) ile fon akışı analizi
SyntheticDataGenerator : Test amaçlı sentetik cüzdan ve işlem üretici

Bağımlılıklar: islem.py → Transaction  |  dugum.py → Wallet
"""

from __future__ import annotations

import random
import string
from collections import deque
from datetime import datetime, timedelta
from typing import Optional

import networkx as nx

from islem import Transaction
from dugum import Wallet


# =============================================================
# BLOKZINCIR AĞI
# =============================================================

class BlockchainNetwork:
    """
    Blokzincir işlem ağını yöneten ana sınıf.

    Veri Yapıları
    -------------
    graph   : networkx.DiGraph
                Düğümler = cüzdan adresleri
                Kenarlar = transferler (weight, count, tx_ids özellikleri taşır)
                Yön      = para akışı yönü (gönderici → alıcı)

    wallets : dict[str, Wallet]  (hash tablosu)
                Adres → Wallet eşlemesi; O(1) erişim sağlar.

    transactions : list[Transaction]
                Zaman sıralı tüm işlem listesi.
    """

    def __init__(self) -> None:
        # Yönlü graf: düğüm = cüzdan, kenar = transfer
        self.graph: nx.DiGraph = nx.DiGraph()
        # Hash tablosu: adres → Wallet nesnesi (O(1) erişim)
        self.wallets: dict[str, Wallet] = {}
        self.transactions: list[Transaction] = []

    # ---- Cüzdan Yönetimi ----

    def add_wallet(self, address: str, initial_balance: float = 0.0) -> None:
        """
        Ağa yeni bir cüzdan ekler.

        Hem hash tablosuna (O(1) erişim için) hem de
        grafa (görselleştirme için) kaydeder.
        """
        wallet = Wallet(address, initial_balance)
        self.wallets[address] = wallet                          # Hash tablosuna ekle
        self.graph.add_node(address, balance=initial_balance)  # Grafa düğüm ekle

    def get_wallet(self, address: str) -> Optional[Wallet]:
        """Hash tablosundan O(1) hızında cüzdan döndürür."""
        return self.wallets.get(address)

    # ---- İşlem Yönetimi ----

    def add_transaction(self, tx: Transaction) -> None:
        """
        İşlemi ağa ekler ve ilgili tüm yapıları günceller.

        Adımlar
        -------
        1. İşlemi transactions listesine ekle.
        2. Gönderici bakiyesini azalt, alıcı bakiyesini artır.
        3. Graf kenarını ekle (veya mevcutsa ağırlığını güncelle).
        4. Graf düğümlerinin 'balance' özelliğini senkronize et.
        """
        self.transactions.append(tx)

        # Bakiyeleri güncelle — hash tablosu üzerinden O(1) erişim
        sender_wallet = self.wallets.get(tx.sender)
        receiver_wallet = self.wallets.get(tx.receiver)

        if sender_wallet:
            sender_wallet.send(tx)
        if receiver_wallet:
            receiver_wallet.receive(tx)

        # Aynı çift arasında önceden kenar varsa toplamı ve sayacı güncelle
        if self.graph.has_edge(tx.sender, tx.receiver):
            self.graph[tx.sender][tx.receiver]["weight"] += tx.amount
            self.graph[tx.sender][tx.receiver]["count"] += 1
            self.graph[tx.sender][tx.receiver]["tx_ids"].append(tx.tx_id)
        else:
            self.graph.add_edge(
                tx.sender,
                tx.receiver,
                weight=round(tx.amount, 6),
                count=1,
                tx_ids=[tx.tx_id],
            )

        # Graf düğümlerinin bakiye özelliğini güncelle
        if tx.sender in self.graph.nodes and sender_wallet:
            self.graph.nodes[tx.sender]["balance"] = sender_wallet.balance
        if tx.receiver in self.graph.nodes and receiver_wallet:
            self.graph.nodes[tx.receiver]["balance"] = receiver_wallet.balance

    # ---- Graf Metrikleri ----

    def average_degree(self) -> float:
        """Ortalama düğüm bağlantı sayısını (derece) hesaplar."""
        if not self.graph:
            return 0.0
        degrees = dict(self.graph.degree()).values()
        return round(sum(degrees) / len(self.graph), 2)

    def total_volume(self) -> float:
        """Ağdaki toplam transfer hacmini (BTC) hesaplar."""
        return round(sum(tx.amount for tx in self.transactions), 4)


# =============================================================
# FON AKIŞI ANALİZÖRÜ (BFS + DFS)
# =============================================================

class FundFlowAnalyzer:
    """
    Bir başlangıç cüzdanından itibaren fonların izlediği yolu analiz eder.

    BFS — Genişlik Öncelikli Arama
    --------------------------------
    Veri yapısı : collections.deque (Kuyruk / Queue)
    Davranış    : Tüm komşuları aynı derinlikte ziyaret eder.
                  Para akışının "katman katman" yayılımını gösterir.

    DFS — Derinlik Öncelikli Arama
    --------------------------------
    Veri yapısı : Python list (Yığıt / Stack)
    Davranış    : Bir dalı sonuna kadar takip eder.
                  Derin ve doğrusal para akışı zincirlerini ortaya çıkarır.
    """

    def __init__(self, network: BlockchainNetwork) -> None:
        self.network = network

    def bfs_fund_flow(self, start: str, max_depth: int = 4) -> dict:
        """
        BFS ile fon akışını izler.

        Kuyruk (deque) kullanarak FIFO (ilk giren ilk çıkar) sırasıyla
        komşuları keşfeder; her adımda derinliği bir artırır.

        Parameters
        ----------
        start     : Başlangıç cüzdan adresi
        max_depth : Kaç katman derinliğe gidileceği (varsayılan: 4)

        Returns
        -------
        {
            "nodes" : Ziyaret edilen tüm düğümler (liste),
            "edges" : Gezilen kenarlar [(kaynak, hedef), ...],
            "order" : Ziyaret sırası (liste)
        }
        """
        if start not in self.network.graph:
            return {"nodes": [], "edges": [], "order": []}

        visited: set[str] = set()
        traversal_order: list[str] = []
        visited_edges: list[tuple] = []

        # Kuyruk: her eleman (cüzdan_adresi, mevcut_derinlik) ikilisidir
        queue: deque = deque()
        queue.append((start, 0))
        visited.add(start)
        traversal_order.append(start)

        while queue:
            current, depth = queue.popleft()  # FIFO — ilk giren ilk çıkar

            if depth >= max_depth:
                continue

            # Giden komşuları (para aktardığı cüzdanları) keşfet
            for neighbor in self.network.graph.successors(current):
                visited_edges.append((current, neighbor))
                if neighbor not in visited:
                    visited.add(neighbor)
                    traversal_order.append(neighbor)
                    queue.append((neighbor, depth + 1))

        return {
            "nodes": list(visited),
            "edges": visited_edges,
            "order": traversal_order,
        }

    def dfs_fund_flow(self, start: str, max_depth: int = 4) -> dict:
        """
        DFS ile fon akışını izler.

        Yığıt (list) kullanarak LIFO (son giren ilk çıkar) sırasıyla
        komşuları keşfeder; her dalı sonuna kadar takip eder.

        Parameters
        ----------
        start     : Başlangıç cüzdan adresi
        max_depth : Kaç seviye derinliğe gidileceği (varsayılan: 4)

        Returns
        -------
        {
            "nodes" : Ziyaret edilen tüm düğümler (liste),
            "edges" : Gezilen kenarlar [(kaynak, hedef), ...],
            "order" : Ziyaret sırası (liste)
        }
        """
        if start not in self.network.graph:
            return {"nodes": [], "edges": [], "order": []}

        visited: set[str] = set()
        traversal_order: list[str] = []
        visited_edges: list[tuple] = []

        # Yığıt: her eleman (cüzdan_adresi, mevcut_derinlik) ikilisidir
        stack: list = [(start, 0)]

        while stack:
            current, depth = stack.pop()  # LIFO — son giren ilk çıkar

            if current in visited:
                continue

            visited.add(current)
            traversal_order.append(current)

            if depth >= max_depth:
                continue

            # Komşuları ters sırada yığıta ekle → doğal soldan sağa sıra korunur
            for neighbor in reversed(
                list(self.network.graph.successors(current))
            ):
                if neighbor not in visited:
                    visited_edges.append((current, neighbor))
                    stack.append((neighbor, depth + 1))

        return {
            "nodes": list(visited),
            "edges": visited_edges,
            "order": traversal_order,
        }


# =============================================================
# SENTETİK VERİ ÜRETİCİ
# =============================================================

class SyntheticDataGenerator:
    """
    Test ve demonstrasyon amaçlı sentetik blokzincir verisi üretir.

    Üretilen veri
    -------------
    - num_wallets adet benzersiz cüzdan adresi
    - num_transactions adet rastgele yönlü transfer işlemi
    - Tekrar üretilebilirlik için seed parametresi desteklenir
    """

    @staticmethod
    def _random_address(index: int) -> str:
        """Bitcoin benzeri kısa ve okunabilir cüzdan adresi üretir."""
        suffix = "".join(
            random.choices(string.ascii_uppercase + string.digits, k=8)
        )
        return f"W{index:02d}_{suffix}"

    @classmethod
    def generate(
        cls,
        num_wallets: int = 18,
        num_transactions: int = 45,
        seed: int = 42,
    ) -> BlockchainNetwork:
        """
        Belirtilen parametre değerleriyle dolu bir BlockchainNetwork döndürür.

        Parameters
        ----------
        num_wallets      : Üretilecek cüzdan sayısı (önerilen: 15–20)
        num_transactions : Üretilecek işlem sayısı (önerilen: 40–50)
        seed             : Rastgele sayı üreteci tohumu (tekrarlanabilirlik için)

        Returns
        -------
        Hazır BlockchainNetwork nesnesi
        """
        random.seed(seed)
        network = BlockchainNetwork()

        # ---- Cüzdanları oluştur ----
        addresses: list[str] = []
        for i in range(num_wallets):
            addr = cls._random_address(i + 1)
            # Başlangıç bakiyesi: 0.5 – 100 BTC (geniş dağılım)
            initial = round(random.uniform(0.5, 100.0), 4)
            network.add_wallet(addr, initial)
            addresses.append(addr)

        # ---- İşlemleri oluştur ----
        # Son 30 günü kapsayan zaman penceresi
        base_time = datetime.now() - timedelta(days=30)

        for _ in range(num_transactions):
            sender = random.choice(addresses)
            # Gönderici ile alıcının farklı olmasını garanti et
            receiver = random.choice(
                [a for a in addresses if a != sender]
            )
            amount = round(random.uniform(0.01, 8.0), 4)
            ts = base_time + timedelta(
                hours=random.randint(0, 720),
                minutes=random.randint(0, 59),
            )
            tx = Transaction(sender, receiver, amount, ts)
            network.add_transaction(tx)

        return network
