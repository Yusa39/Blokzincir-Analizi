using System;
using System.Security.Cryptography;
using System.Text;

namespace BlokzincirAnalizi
{
    public class Islem
    {
        // İşlemin temel özellikleri (Kimden, Kime, Ne Kadar, Ne Zaman)
        public string Gonderen { get; set; }
        public string Alici { get; set; }
        public decimal Miktar { get; set; }
        public DateTime Tarih { get; set; }

        // İşlemin değiştirilemez dijital parmak izi
        public string IslemHash { get; set; }

        // Sınıfın yapıcı metodu (Yeni bir işlem oluşturulduğunda çalışır)
        public Islem(string gonderen, string alici, decimal miktar)
        {
            Gonderen = gonderen;
            Alici = alici;
            Miktar = miktar;
            Tarih = DateTime.Now; // İşlemin yapıldığı anı otomatik kaydeder
            IslemHash = HashHesapla(); // Veriler girilir girilmez şifreyi oluşturur
        }

        // Güvenlik için SHA256 algoritması ile verileri şifreleyen (Hash'leyen) metot
        public string HashHesapla()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Tüm verileri yan yana birleştiriyoruz
                string hamVeri = $"{Gonderen}-{Alici}-{Miktar}-{Tarih}";

                // Birleşmiş metni şifreli kriptografik bir bayt dizisine çeviriyoruz
                byte[] byteDizisi = sha256.ComputeHash(Encoding.UTF8.GetBytes(hamVeri));

                // Okunabilir bir metne (String) dönüştürüp geri döndürüyoruz
                return BitConverter.ToString(byteDizisi).Replace("-", "").ToLower();
            }
        }
    }
}