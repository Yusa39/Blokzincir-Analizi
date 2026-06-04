namespace BlokzincirAnalizi
{
    /// <summary>
    /// Blokzincir ağındaki bir cüzdanı (graf düğümü) temsil eder.
    ///
    /// Bakiye hesabı:
    ///   bakiye = başlangıç_bakiyesi
    ///            + Σ(gelen transfer miktarları)
    ///            − Σ(giden transfer miktarları)
    ///
    /// Cüzdan nesneleri, BlokzincirAgi sınıfındaki bir Dictionary
    /// (hash tablosu) içinde tutularak O(1) hızında adres → Cuzdan
    /// erişimi sağlanır.
    /// </summary>
    public class Cuzdan
    {
        /// <summary>Benzersiz cüzdan adresi (hash tablosu anahtarı).</summary>
        public string Adres { get; }

        /// <summary>Anlık bakiye (BTC).</summary>
        public decimal Bakiye { get; private set; }

        /// <summary>Bu cüzdana yapılmış gelen işlemler.</summary>
        public List<Islem> GelenIslemler { get; } = [];

        /// <summary>Bu cüzdandan yapılmış giden işlemler.</summary>
        public List<Islem> GidenIslemler { get; } = [];

        /// <param name="adres">Benzersiz cüzdan adresi</param>
        /// <param name="baslangicBakiye">Başlangıç BTC bakiyesi (varsayılan: 0)</param>
        public Cuzdan(string adres, decimal baslangicBakiye = 0m)
        {
            Adres = adres;
            Bakiye = Math.Round(baslangicBakiye, 6);
        }

        // ---- Bakiye Güncelleme ----

        /// <summary>Gelen işlemi kaydeder; bakiyeyi artırır.</summary>
        public void AlGelen(Islem islem)
        {
            GelenIslemler.Add(islem);
            Bakiye = Math.Round(Bakiye + islem.Miktar, 6);
        }

        /// <summary>Giden işlemi kaydeder; bakiyeyi azaltır.</summary>
        public void GonderGiden(Islem islem)
        {
            GidenIslemler.Add(islem);
            Bakiye = Math.Round(Bakiye - islem.Miktar, 6);
        }

        // ---- Özet ----

        /// <summary>Toplam gelen BTC miktarını hesaplar.</summary>
        public decimal ToplamGelen => GelenIslemler.Sum(i => i.Miktar);

        /// <summary>Toplam giden BTC miktarını hesaplar.</summary>
        public decimal ToplamGiden => GidenIslemler.Sum(i => i.Miktar);

        public override string ToString() =>
            $"{Adres}  Bakiye: {Bakiye:F4} BTC  ↓{GelenIslemler.Count} ↑{GidenIslemler.Count}";
    }
}