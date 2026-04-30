namespace BlokzincirAnalizi
{
    /// <summary>
    /// Blokzincir işlem ağını yönetir.
    ///
    /// Veri Yapıları:
    ///   _cuzdanlar : Dictionary&lt;string, Cuzdan&gt; (hash tablosu)
    ///                Adres → Cüzdan eşlemesi; O(1) erişim sağlar.
    ///
    ///   _kenarlar  : Dictionary&lt;string, List&lt;KenarBilgisi&gt;&gt;
    ///                Komşuluk listesi (adjacency list) → yönlü graf.
    ///                Her gönderici adres, gönderdiği (hedef, işlem) çiftlerini tutar.
    ///                Para akışı yönü: gönderici → alıcı.
    /// </summary>
    public class BlokzincirAgi
    {
        // Hash tablosu: adres → Cüzdan nesnesi (O(1) erişim)
        private readonly Dictionary<string, Cuzdan> _cuzdanlar = [];

        // Komşuluk listesi (adjacency list): kaynak → Liste<(hedef, işlem)>
        private readonly Dictionary<string, List<KenarBilgisi>> _kenarlar = [];

        // Tüm işlem listesi
        private readonly List<Islem> _islemler = [];

        // ---- Salt okunur erişim ----

        public IReadOnlyDictionary<string, Cuzdan> Cuzdanlar => _cuzdanlar;
        public IReadOnlyList<Islem> Islemler => _islemler;

        // ---- Cüzdan Yönetimi ----

        /// <summary>
        /// Ağa yeni bir cüzdan ekler.
        /// Hash tablosuna ve komşuluk listesine birlikte eklenir.
        /// </summary>
        public void CuzdanEkle(string adres, decimal baslangicBakiye = 0m)
        {
            _cuzdanlar[adres] = new Cuzdan(adres, baslangicBakiye); // Hash tablosuna ekle
            _kenarlar[adres]  = [];                                  // Komşuluk listesi başlat
        }

        /// <summary>Hash tablosundan O(1) hızında cüzdan döndürür.</summary>
        public Cuzdan? CuzdanGetir(string adres) =>
            _cuzdanlar.TryGetValue(adres, out var c) ? c : null;

        // ---- İşlem Yönetimi ----

        /// <summary>
        /// İşlemi ağa ekler ve ilgili tüm yapıları günceller:
        ///   1. İşlemi _islemler listesine ekler.
        ///   2. Gönderici/alıcı bakiyelerini günceller (hash tablosu O(1)).
        ///   3. Komşuluk listesine kenar olarak ekler.
        /// </summary>
        public void IslemEkle(Islem islem)
        {
            _islemler.Add(islem);

            // Bakiyeleri güncelle — hash tablosu üzerinden O(1) erişim
            if (_cuzdanlar.TryGetValue(islem.Gonderen, out var gonderen))
                gonderen.GonderGiden(islem);
            if (_cuzdanlar.TryGetValue(islem.Alici, out var alici))
                alici.AlGelen(islem);

            // Komşuluk listesine kenar ekle (gönderici yoksa başlat)
            if (!_kenarlar.ContainsKey(islem.Gonderen))
                _kenarlar[islem.Gonderen] = [];

            _kenarlar[islem.Gonderen].Add(new KenarBilgisi(islem.Alici, islem));
        }

        // ---- Graf Sorgulama ----

        /// <summary>Bir düğümün giden komşularını (hedef, işlem) çiftleri olarak döndürür.</summary>
        public IEnumerable<KenarBilgisi> KomsuGetir(string adres) =>
            _kenarlar.TryGetValue(adres, out var liste) ? liste : [];

        /// <summary>Graftaki tüm düğüm adreslerini döndürür.</summary>
        public IEnumerable<string> TumDugumler() => _cuzdanlar.Keys;

        // ---- Metrikler ----

        /// <summary>Ağdaki toplam transfer hacmini (BTC) hesaplar.</summary>
        public decimal ToplamHacim() => _islemler.Sum(i => i.Miktar);

        /// <summary>Toplam kenar sayısını döndürür.</summary>
        public int ToplamKenarSayisi() => _kenarlar.Values.Sum(l => l.Count);

        /// <summary>Ortalama çıkış derecesini (giden kenar ortalaması) hesaplar.</summary>
        public double OrtalamaDerecesi()
        {
            if (_cuzdanlar.Count == 0) return 0;
            return _kenarlar.Values.Average(l => (double)l.Count);
        }
    }

    /// <summary>Komşuluk listesindeki bir kenar girişini temsil eder.</summary>
    public record KenarBilgisi(string Hedef, Islem Islem);
}
