#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
dugum.py
--------
Kripto ağındaki bir cüzdanı (düğümü) temsil eden Wallet sınıfı.

Cüzdan nesneleri dışarıda bir hash tablosunda (Python dict) tutularak
adres → Wallet eşlemesine O(1) hızında erişim sağlanır.

Bağımlılık: islem.py → Transaction
"""

from __future__ import annotations
from islem import Transaction


class Wallet:
    """
    Bir kripto cüzdanını temsil eder.

    Bakiye Hesabı
    -------------
    bakiye = başlangıç_bakiyesi
             + Σ(gelen transfer miktarları)
             − Σ(giden transfer miktarları)

    Özellikler
    ----------
    address  : Benzersiz cüzdan adresi (string anahtar)
    balance  : Anlık bakiye (BTC)
    incoming : Bu cüzdana yapılan Transaction nesneleri listesi
    outgoing : Bu cüzdandan yapılan Transaction nesneleri listesi
    """

    def __init__(self, address: str, initial_balance: float = 0.0) -> None:
        self.address: str = address
        self.balance: float = round(initial_balance, 6)

        # Gelen ve giden işlem geçmişleri
        self.incoming: list[Transaction] = []
        self.outgoing: list[Transaction] = []

    # ---- Bakiye Güncelleme ----

    def receive(self, tx: Transaction) -> None:
        """Gelen bir işlemi kaydeder; bakiyeyi artırır."""
        self.incoming.append(tx)
        self.balance = round(self.balance + tx.amount, 6)

    def send(self, tx: Transaction) -> None:
        """Giden bir işlemi kaydeder; bakiyeyi azaltır."""
        self.outgoing.append(tx)
        self.balance = round(self.balance - tx.amount, 6)

    # ---- Özet ve Serileştirme ----

    def summary(self) -> dict:
        """
        Cüzdanın anlık özetini sözlük olarak döndürür.
        Streamlit tablolarında doğrudan kullanılabilir.
        """
        return {
            "Adres": self.address,
            "Bakiye (BTC)": self.balance,
            "Gelen İşlem": len(self.incoming),
            "Giden İşlem": len(self.outgoing),
            "Toplam Gelen (BTC)": round(
                sum(t.amount for t in self.incoming), 6
            ),
            "Toplam Giden (BTC)": round(
                sum(t.amount for t in self.outgoing), 6
            ),
        }

    def __repr__(self) -> str:
        return (
            f"Wallet(address={self.address!r}, "
            f"balance={self.balance}, "
            f"in={len(self.incoming)}, out={len(self.outgoing)})"
        )
