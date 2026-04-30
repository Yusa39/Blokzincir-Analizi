using System.Security.Cryptography;
using System.Text;

namespace BlokzincirAnalizi
{
    /// <summary>
    /// Blokzincirdeki tek bir transfer işlemini temsil eder.
    ///
    /// TxID otomatik üretim:
    ///   SHA-256(gönderici + alıcı + miktar + ISO zaman damgası)
    ///   → İlk 16 hex karakteri TxID olarak kullanılır.
    /// </summary>
    public class Islem
    {
        /// <summary>SHA-256 ile türetilmiş benzersiz işlem kimliği (16 karakter).</summary>
        public string TxId { get; }

        /// <summary>Gönderici cüzdan adresi.</summary>
        public string Gonderen { get; }

        /// <summary>Alıcı cüzdan adresi.</summary>
        public string Alici { get; }

        /// <summary>Transfer miktarı (BTC, 6 ondalık basamak).</summary>
        public decimal Miktar { get; }

        /// <summary>İşlem zamanı.</summary>
        public DateTime Zaman { get; }

        /// <param name="gonderen">Gönderici cüzdan adresi</param>
        /// <param name="alici">Alıcı cüzdan adresi</param>
        /// <param name="miktar">Transfer miktarı (BTC)</param>
        /// <param name="zaman">İşlem gerçekleşme zamanı</param>
        public Islem(string gonderen, string alici, decimal miktar, DateTime zaman)
        {
            Gonderen = gonderen;
            Alici    = alici;
            Miktar   = Math.Round(miktar, 6);
            Zaman    = zaman;

            // TxID = SHA-256(gönderici + alıcı + miktar + ISO-8601 zaman)
            string ham = $"{gonderen}{alici}{miktar}{zaman:O}";
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(ham));
            TxId = Convert.ToHexString(hash)[..16].ToLower();
        }

        public override string ToString() =>
            $"[{TxId}] {Gonderen} → {Alici} : {Miktar:F4} BTC  {Zaman:yyyy-MM-dd HH:mm}";
    }
}
