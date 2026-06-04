using System;
using System.Security.Cryptography;
using System.Text;

namespace BlokzincirAnalizi
{
    public class Islem
    {
        // --- ORTAK ÖZELLİKLER ---
        public string Gonderen { get; set; }
        public string Alici { get; set; }
        public decimal Miktar { get; set; }

        // --- AŞAMA 1 İÇİN GEREKLİ ALANLAR ---
        public DateTime Tarih { get; set; }
        public string IslemHash { get; set; }

        // --- AŞAMA 2 (MEHMET AKİF) İÇİN GEREKLİ ALANLAR ---
        public DateTime Zaman => Tarih; // Zaman denildiğinde otomatik Tarih'i döndürür
        public string TxId => IslemHash; // TxId denildiğinde otomatik IslemHash'i döndürür

        // ======================================================================
        // YAPICI METOT 1: Senin Aşama 1 kodlarının çalışmasını sağlayan constructor
        // ======================================================================
        public Islem(string gonderen, string alici, decimal miktar)
        {
            Gonderen = gonderen;
            Alici = alici;
            Miktar = miktar;
            Tarih = DateTime.Now;
            IslemHash = HashHesapla();
        }

        // ======================================================================
        // YAPICI METOT 2: Arkadaşının Aşama 2 (SentetikVeriUretici) kodlarını çalıştıran constructor
        // ======================================================================
        public Islem(string gonderen, string alici, decimal miktar, DateTime zaman)
        {
            Gonderen = gonderen;
            Alici = alici;
            Miktar = Math.Round(miktar, 6);
            Tarih = zaman; // Gelen zamanı bizim ortak Tarih alanımıza kaydediyoruz

            // Arkadaşının TxId mantığına göre şifre üretimi
            string ham = $"{gonderen}{alici}{miktar}{zaman:O}";
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(ham));
            IslemHash = BitConverter.ToString(hash).Replace("-", "").Substring(0, 16).ToLower();
        }

        // Şifreleme Metodu (Aşama 1 için)
        public string HashHesapla()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string hamVeri = $"{Gonderen}-{Alici}-{Miktar}-{Tarih}";
                byte[] byteDizisi = sha256.ComputeHash(Encoding.UTF8.GetBytes(hamVeri));
                return BitConverter.ToString(byteDizisi).Replace("-", "").ToLower();
            }
        }

        // Arkadaşının eklediği ToString() ezmesi
        public override string ToString() =>
            $"[{TxId}] {Gonderen} → {Alici} : {Miktar:F4} BTC  {Zaman:yyyy-MM-dd HH:mm}";
    }
}