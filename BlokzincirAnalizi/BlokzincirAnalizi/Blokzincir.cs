using System;
using System.Collections.Generic;

namespace BlokzincirAnalizi
{
    public class Blokzincir
    {
        // Tüm düğümleri (blokları) arka arkaya tutacağımız ana liste (ZİNCİR)
        public List<Dugum> Zincir { get; set; }

        // Sistem ilk çalıştığında ne olacak?
        public Blokzincir()
        {
            Zincir = new List<Dugum>();
            // Zincir boş kalamaz, yaratılış (Genesis) düğümünü ekliyoruz
            Zincir.Add(IlkSifirinciDugumuOlustur());
        }

        // 1. Yaratılış (Genesis) Düğümü Oluşturucu
        // Blokzincirlerin 0. bloğunun öncesi olmadığı için manuel oluşturulur
        private Dugum IlkSifirinciDugumuOlustur()
        {
            List<Islem> baslangicIslemleri = new List<Islem>
            {
                new Islem("Kurucu", "Sistem", 0)
            };

            // Önceki hash olmadığı için sıfırlardan oluşan sembolik bir şifre veriyoruz
            return new Dugum(0, baslangicIslemleri, "0000000000000000000000000000000000000000000000000000000000000000");
        }

        // 2. Zincirdeki En Son Düğümü Getir
        public Dugum SonDugumuGetir()
        {
            return Zincir[Zincir.Count - 1];
        }

        // 3. Zincire Yeni Düğüm Ekleme Motoru
        public void DugumEkle(Dugum yeniDugum)
        {
            // Yeni düğümün 'ÖncekiHash' değerini, zincirdeki son düğümün 'Hash'ine eşitliyoruz (Zincirleme kancası takıldı!)
            yeniDugum.OncekiHash = SonDugumuGetir().Hash;
            yeniDugum.Indeks = SonDugumuGetir().Indeks + 1;

            // Bilgiler (ÖncekiHash ve Indeks) değiştiği için yeni düğümün kendi şifresini son bir kez güncelliyoruz
            yeniDugum.Hash = yeniDugum.HashHesapla();

            Zincir.Add(yeniDugum);
        }

        // 4. Zincir Güvenlik Kontrolü (Sistem Hacklendi mi?)
        public bool ZincirGecerliMi()
        {
            // 0. düğüm (Genesis) manuel olduğu için 1. düğümden kontrol etmeye başlıyoruz
            for (int i = 1; i < Zincir.Count; i++)
            {
                Dugum suAnkiDugum = Zincir[i];
                Dugum oncekiDugum = Zincir[i - 1];

                // Kural 1: Düğümün içindeki veriler değiştirilmiş mi? (Miktar vs. değişirse hash farklı çıkar)
                if (suAnkiDugum.Hash != suAnkiDugum.HashHesapla())
                    return false;

                // Kural 2: Geçmişle bağlantı kopmuş mu?
                if (suAnkiDugum.OncekiHash != oncekiDugum.Hash)
                    return false;
            }
            return true; // Tüm testleri geçerse zincir güvenlidir!
        }
    }
}