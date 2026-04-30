using System.Security.Cryptography;
using System.Text;

namespace BlokzincirAnalizi
{
    /// <summary>
    /// SHA-256 tabanlı Merkle Ağacı.
    ///
    /// Blokzincirde bir işlem grubunun bütünlüğünü tek bir
    /// "Merkle Root" hash değeriyle özetlemek için kullanılır.
    /// Herhangi bir işlem değiştirildiğinde kök hash tamamen değişir.
    ///
    /// Algoritma (özyinelemeli):
    ///   1. Her tx_id'yi SHA-256 ile yaprak hash'e dönüştür.
    ///   2. Çift sayı yoksa son yaprağı kopyala (Bitcoin standardı).
    ///   3. Her komşu çifti birleştirip hash'le → üst seviye.
    ///   4. Tek hash kalana dek tekrarla → Merkle Root.
    /// </summary>
    public class MerkleAgaci
    {
        /// <summary>Nihai Merkle Root hash değeri.</summary>
        public string Kok { get; private set; } = string.Empty;

        /// <summary>Yaprak düğümleri: her tx_id'nin SHA-256 hash'i.</summary>
        public List<string> Yapraklar { get; private set; } = [];

        /// <summary>Her seviyenin hash listesini saklar (görselleştirme için).</summary>
        public List<List<string>> Seviyeler { get; private set; } = [];

        private readonly List<Islem> _islemler;

        /// <param name="islemler">Merkle ağacına dahil edilecek işlem listesi</param>
        public MerkleAgaci(List<Islem> islemler)
        {
            _islemler = islemler;
            Yapraklar = islemler.Select(i => Sha256(i.TxId)).ToList();
            Kok = Insa(new List<string>(Yapraklar));
        }

        // ---- Yardımcı Metotlar ----

        /// <summary>Verilen string'i UTF-8 kodlayıp SHA-256 ile hash'ler.</summary>
        private static string Sha256(string veri)
        {
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(veri));
            return Convert.ToHexString(hash).ToLower();
        }

        /// <summary>
        /// Özyinelemeli Merkle ağacı inşası.
        /// Her çağrıda mevcut seviyeyi Seviyeler listesine kaydeder.
        /// </summary>
        private string Insa(List<string> dugumler)
        {
            // Boş işlem listesi durumu
            if (dugumler.Count == 0)
                return Sha256("BOS_AGAC");

            // Mevcut seviyeyi kaydet
            Seviyeler.Add(new List<string>(dugumler));

            // Tek düğüm kaldı → bu düğüm Merkle Root'tur
            if (dugumler.Count == 1)
                return dugumler[0];

            // Tek sayıda düğüm varsa son düğümü kopyala
            if (dugumler.Count % 2 != 0)
                dugumler.Add(dugumler[^1]);

            // İkişerli çiftleri birleştirip hash'le → üst seviye
            var ustSeviye = new List<string>();
            for (int i = 0; i < dugumler.Count; i += 2)
                ustSeviye.Add(Sha256(dugumler[i] + dugumler[i + 1]));

            // Özyinelemeli çağrı: bir üst seviyeyle devam et
            return Insa(ustSeviye);
        }

        // ---- Genel Metotlar ----

        /// <summary>
        /// Verilen tx_id'nin bu Merkle ağacında kayıtlı olup olmadığını doğrular.
        /// tx_id'yi SHA-256 ile hash'leyerek yaprak listesinde arar.
        /// </summary>
        public bool DogrulaIslem(string txId)
        {
            string hash = Sha256(txId);
            return Yapraklar.Contains(hash);
        }

        /// <summary>Ağacın seviye sayısını (derinliğini) döndürür.</summary>
        public int Derinlik => Seviyeler.Count;

        /// <summary>Ağaçtaki toplam işlem sayısı.</summary>
        public int IslemSayisi => _islemler.Count;

        public override string ToString() =>
            $"MerkleAgaci(işlem={IslemSayisi}, derinlik={Derinlik}, kök={Kok[..12]}...)";
    }
}
