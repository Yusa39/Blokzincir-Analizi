using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace BlokzincirAnalizi
{
    public class MerkleAgaci
    {
        // Tüm işlemlerin özetlerini (Hash) liste olarak tutuyoruz
        public static string MerkleRootHesapla(List<Islem> islemler)
        {
            if (islemler == null || !islemler.Any()) return string.Empty;

            // Önce tüm işlemlerin kendi hash'lerini alıyoruz
            List<string> yapraklar = islemler.Select(x => x.IslemHash).ToList();

            return AgacOlustur(yapraklar);
        }

        private static string AgacOlustur(List<string> hashler)
        {
            // Eğer elimizde sadece 1 hash kaldıysa, o artık Merkle Root (Tepe) noktasıdır
            if (hashler.Count == 1) return hashler[0];

            List<string> ustSeviye = new List<string>();

            // Hashleri ikişerli gruplayıp birleştiriyoruz
            for (int i = 0; i < hashler.Count; i += 2)
            {
                if (i + 1 < hashler.Count)
                {
                    // İki komşuyu birleştir ve yeni bir hash üret
                    ustSeviye.Add(HashOlustur(hashler[i] + hashler[i + 1]));
                }
                else
                {
                    // Eğer tek bir tane kaldıysa, onu kendisiyle eşleştirip yukarı taşı (Standart kural)
                    ustSeviye.Add(HashOlustur(hashler[i] + hashler[i]));
                }
            }

            // Bir üst seviyeye çıkıp tekrar aynı işlemi yap (Özyineleme/Recursion)
            return AgacOlustur(ustSeviye);
        }

        private static string HashOlustur(string veri)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] byteDizisi = sha256.ComputeHash(Encoding.UTF8.GetBytes(veri));
                return BitConverter.ToString(byteDizisi).Replace("-", "").ToLower();
            }
        }
    }
}