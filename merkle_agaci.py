#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
merkle_agaci.py
---------------
SHA-256 tabanlı Merkle Ağacı implementasyonu.

Merkle Ağacı, blokzincirde bir işlem grubunun bütünlüğünü
tek bir kök hash (Merkle Root) ile özetlemek için kullanılır.
Herhangi bir işlem değiştirildiğinde kök hash tamamen değişir.

Bağımlılık: islem.py → Transaction
"""

from __future__ import annotations
import hashlib
from islem import Transaction


class MerkleTree:
    """
    SHA-256 tabanlı Merkle Ağacı.

    Yapı
    ----
    - Yapraklar (leaves) : Her Transaction.tx_id'nin SHA-256 hash'i
    - Ara seviyeler      : Komşu çift hash'lerin birleştirilip tekrar hash'lenmesi
    - Kök (root)         : Tüm ağacı özetleyen tek SHA-256 hash değeri

    Algoritma (özyinelemeli)
    ------------------------
    1. tx_id listesini SHA-256 ile yaprak listesine dönüştür.
    2. Çift sayı yoksa son yaprağı kopyala (Bitcoin standardı).
    3. Her çifti birleştirip hash'le → bir üst seviye oluştur.
    4. Tek hash kalana dek tekrarla → Merkle Root.

    Bağımlılık: islem.Transaction
    """

    def __init__(self, transactions: list[Transaction]) -> None:
        self.transactions: list[Transaction] = transactions

        # Yaprak düğümleri: her tx_id'nin SHA-256 hash'i
        self.leaves: list[str] = [
            self._sha256(tx.tx_id) for tx in transactions
        ]

        # Her seviyenin hash listesini saklar (görselleştirme için)
        self.levels: list[list[str]] = []

        # Ağacı inşa et ve kökü hesapla
        self.root: str = self._build(self.leaves.copy())

    # ---- Özel Metotlar ----

    @staticmethod
    def _sha256(data: str) -> str:
        """Verilen string'i UTF-8 kodlayıp SHA-256 ile hash'ler."""
        return hashlib.sha256(data.encode("utf-8")).hexdigest()

    def _build(self, nodes: list[str]) -> str:
        """
        Özyinelemeli Merkle ağacı inşası.

        Her çağrıda mevcut seviyeyi self.levels'a kaydeder,
        bir üst seviyeyi hesaplayarak kendini tekrar çağırır.
        Tek düğüm kalana dek devam eder.

        Parameters
        ----------
        nodes : Mevcut seviyedeki hash listesi

        Returns
        -------
        Merkle Root hash string'i
        """
        # Boş işlem listesi durumu
        if not nodes:
            return self._sha256("EMPTY_TREE")

        # Mevcut seviyeyi kaydet (görselleştirme için)
        self.levels.append(nodes.copy())

        # Tek düğüm kaldı → bu düğüm Merkle Root'tur
        if len(nodes) == 1:
            return nodes[0]

        # Tek sayıda düğüm varsa son düğümü kopyala
        if len(nodes) % 2 != 0:
            nodes.append(nodes[-1])

        # İkişerli çiftleri hash'leyerek bir üst seviye oluştur
        parent_level: list[str] = []
        for i in range(0, len(nodes), 2):
            combined = nodes[i] + nodes[i + 1]
            parent_level.append(self._sha256(combined))

        # Özyinelemeli çağrı: bir üst seviyeyle devam et
        return self._build(parent_level)

    # ---- Genel Metotlar ----

    def verify_transaction(self, tx_id: str) -> bool:
        """
        Verilen tx_id'nin bu Merkle ağacına ait olup olmadığını doğrular.

        tx_id'yi SHA-256 ile hash'leyerek yaprak listesinde arar.

        Parameters
        ----------
        tx_id : Doğrulanmak istenen işlem kimliği

        Returns
        -------
        True  → işlem Merkle ağacında kayıtlı
        False → işlem bulunamadı veya değiştirilmiş
        """
        candidate_hash = self._sha256(tx_id)
        return candidate_hash in self.leaves

    def depth(self) -> int:
        """Ağacın seviye sayısını (derinliğini) döndürür."""
        return len(self.levels)

    def __repr__(self) -> str:
        return (
            f"MerkleTree(transactions={len(self.transactions)}, "
            f"depth={self.depth()}, root={self.root[:12]!r}…)"
        )
