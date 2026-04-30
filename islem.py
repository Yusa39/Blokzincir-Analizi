#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
islem.py
--------
Blokzincir işlemini (transfer) temsil eden Transaction sınıfı.

Her işlem oluşturulurken gönderici, alıcı, miktar ve zaman bilgilerinden
SHA-256 ile benzersiz bir TxID otomatik olarak türetilir.
"""

import hashlib
from datetime import datetime


class Transaction:
    """
    Tek bir blokzincir işlemini (transfer) temsil eder.

    Özellikler
    ----------
    tx_id     : SHA-256 ile hesaplanmış benzersiz işlem kimliği (ilk 16 karakter)
    sender    : Gönderici cüzdan adresi
    receiver  : Alıcı cüzdan adresi
    amount    : Transfer miktarı (BTC cinsinden, 6 ondalık basamak)
    timestamp : İşlem zamanı (datetime nesnesi)
    """

    def __init__(
        self,
        sender: str,
        receiver: str,
        amount: float,
        timestamp: datetime,
    ) -> None:
        self.sender: str = sender
        self.receiver: str = receiver
        self.amount: float = round(amount, 6)
        self.timestamp: datetime = timestamp

        # TxID = SHA-256(gönderici + alıcı + miktar + ISO zaman damgası)
        raw = f"{sender}{receiver}{amount}{timestamp.isoformat()}"
        self.tx_id: str = hashlib.sha256(raw.encode("utf-8")).hexdigest()[:16]

    # ---- Yardımcı Metotlar ----

    def to_dict(self) -> dict:
        """Tablolaştırma ve serileştirme için sözlük döndürür."""
        return {
            "TxID": self.tx_id,
            "Gönderici": self.sender,
            "Alıcı": self.receiver,
            "Miktar (BTC)": self.amount,
            "Zaman": self.timestamp.strftime("%Y-%m-%d %H:%M"),
        }

    def __repr__(self) -> str:
        return (
            f"Transaction(tx_id={self.tx_id!r}, "
            f"sender={self.sender!r}, receiver={self.receiver!r}, "
            f"amount={self.amount})"
        )
