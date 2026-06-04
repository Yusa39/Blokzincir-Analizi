using deneme;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BlokzincirAnalizi
{
    public class Dugum
    {
        // Bloğun kimlik bilgileri
        public int Indeks { get; set; } // Zincirdeki sırası (0, 1, 2...)
        public DateTime Tarih { get; set; } // Bloğun oluşturulduğu an

        // İŞTE BAĞLANTI BURADA: Az önce oluşturduğun Islem sınıfından bir liste!
        public List<Islem> Islemler { get; set; }

        // Zincirleme için gereken şifreler
        public string OncekiHash { get; set; } // Kendinden bir önceki bloğun şifresi
        public string Hash { get; set; } // Bu bloğun kendi dijital parmak izi

        // Yapıcı Metot (Yeni bir blok oluşturulduğunda çalışır)
        public Dugum(int indeks, List<Islem> islemler, string oncekiHash = "")
        {
            Indeks = indeks;
            Tarih = DateTime.Now;
            Islemler = islemler;
            OncekiHash = oncekiHash;
            Hash = HashHesapla(); // Veriler girilince kendi şifresini üretir
        }

        // Bloğun içindeki tüm verileri ve İŞLEMLERİ harmanlayıp şifreleyen metot
        public string HashHesapla()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // 1. Foreach döngüsünü tamamen sildik! 
                // 2. Tüm işlemlerin listesini tek seferde Merkle Ağacına verip şifreyi alıyoruz.
                string merkleRoot = MerkleAgaci.MerkleRootHesapla(Islemler);

                // Bloğun kendi bilgileriyle, işlemlerin Merkle özetini birleştiriyoruz
                string hamVeri = $"{Indeks}-{Tarih}-{OncekiHash}-{merkleRoot}";

                byte[] byteDizisi = sha256.ComputeHash(Encoding.UTF8.GetBytes(hamVeri));
                return BitConverter.ToString(byteDizisi).Replace("-", "").ToLower();
            }
        }    }
}