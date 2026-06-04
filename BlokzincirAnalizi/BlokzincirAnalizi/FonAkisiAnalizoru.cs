namespace BlokzincirAnalizi
{
    /// <summary>
    /// Blokzincir ağında fon akışı analizi yapar.
    ///
    /// BFS — Genişlik Öncelikli Arama (Breadth-First Search)
    ///   Veri yapısı : Queue&lt;T&gt; (Kuyruk) — FIFO, ilk giren ilk çıkar.
    ///   Davranış    : Tüm komşuları aynı derinlikte ziyaret eder.
    ///                 Paranın "dalgalar halinde" nasıl yayıldığını gösterir.
    ///
    /// DFS — Derinlik Öncelikli Arama (Depth-First Search)
    ///   Veri yapısı : Stack&lt;T&gt; (Yığıt) — LIFO, son giren ilk çıkar.
    ///   Davranış    : Bir dalı sonuna kadar takip eder.
    ///                 Uzun para transferi zincirlerini ortaya çıkarır.
    /// </summary>
    public class FonAkisiAnalizoru
    {
        private readonly BlokzincirAgi _ag;

        public FonAkisiAnalizoru(BlokzincirAgi ag) => _ag = ag;

        // ============================================================
        // BFS — Kuyruk (Queue) ile Genişlik Öncelikli Arama
        // ============================================================

        /// <summary>
        /// BFS ile belirtilen cüzdandan başlayarak fon akışını izler.
        ///
        /// Queue (kuyruk) FIFO sırasıyla çalışır:
        ///   Enqueue → kuyruğun sonuna ekle
        ///   Dequeue → kuyruğun başından al
        /// Her derinlik seviyesindeki tüm komşular tam olarak işlenir,
        /// sonra bir sonraki derinlik seviyesine geçilir.
        /// </summary>
        /// <param name="baslangic">Başlangıç cüzdan adresi</param>
        /// <param name="maxDerinlik">Kaç katman derinliğe gidileceği (varsayılan: 4)</param>
        public AnalizSonucu BfsFonAkisi(string baslangic, int maxDerinlik = 4)
        {
            if (!_ag.TumDugumler().Contains(baslangic))
                return AnalizSonucu.Bos("BFS", baslangic);

            var ziyaretEdilen = new HashSet<string>();
            var kenarlar = new List<(string Kaynak, string Hedef)>();
            var sira = new List<string>();

            // Kuyruk: her eleman (adres, mevcut derinlik) ikilisidir
            var kuyruk = new Queue<(string Adres, int Derinlik)>();

            kuyruk.Enqueue((baslangic, 0)); // Başlangıç düğümünü kuyruğa ekle
            ziyaretEdilen.Add(baslangic);
            sira.Add(baslangic);

            while (kuyruk.Count > 0)
            {
                // FIFO: kuyruğun BAŞINDAN al
                var (mevcutAdres, derinlik) = kuyruk.Dequeue();

                if (derinlik >= maxDerinlik)
                    continue;

                // Bu düğümün giden tüm komşularını keşfet
                foreach (var kenar in _ag.KomsuGetir(mevcutAdres))
                {
                    kenarlar.Add((mevcutAdres, kenar.Hedef));

                    if (!ziyaretEdilen.Contains(kenar.Hedef))
                    {
                        ziyaretEdilen.Add(kenar.Hedef);
                        sira.Add(kenar.Hedef);
                        kuyruk.Enqueue((kenar.Hedef, derinlik + 1));
                    }
                }
            }

            return new AnalizSonucu
            {
                ZiyaretEdilenDugumler = [.. ziyaretEdilen],
                ZiyaretEdilenKenarlar = kenarlar,
                ZiyaretSirasi = sira,
                Algoritma = "BFS",
                Baslangic = baslangic,
                MaxDerinlik = maxDerinlik,
            };
        }

        // ============================================================
        // DFS — Yığıt (Stack) ile Derinlik Öncelikli Arama
        // ============================================================

        /// <summary>
        /// DFS ile belirtilen cüzdandan başlayarak fon akışını izler.
        ///
        /// Stack (yığıt) LIFO sırasıyla çalışır:
        ///   Push → yığıtın ÜSTÜNE ekle
        ///   Pop  → yığıtın ÜSTÜNDEN al
        /// Bir dalı sonuna kadar takip eder, sonra geri döner.
        /// </summary>
        /// <param name="baslangic">Başlangıç cüzdan adresi</param>
        /// <param name="maxDerinlik">Kaç seviye derinliğe gidileceği (varsayılan: 4)</param>
        public AnalizSonucu DfsFonAkisi(string baslangic, int maxDerinlik = 4)
        {
            if (!_ag.TumDugumler().Contains(baslangic))
                return AnalizSonucu.Bos("DFS", baslangic);

            var ziyaretEdilen = new HashSet<string>();
            var kenarlar = new List<(string Kaynak, string Hedef)>();
            var sira = new List<string>();

            // Yığıt: her eleman (adres, mevcut derinlik) ikilisidir
            var yigit = new Stack<(string Adres, int Derinlik)>();

            yigit.Push((baslangic, 0)); // Başlangıç düğümünü yığıta ekle

            while (yigit.Count > 0)
            {
                // LIFO: yığıtın ÜSTÜNDEN al
                var (mevcutAdres, derinlik) = yigit.Pop();

                if (ziyaretEdilen.Contains(mevcutAdres))
                    continue;

                ziyaretEdilen.Add(mevcutAdres);
                sira.Add(mevcutAdres);

                if (derinlik >= maxDerinlik)
                    continue;

                // Komşuları TERS sırada yığıta ekle → doğal soldan sağa ziyaret sırası korunur
                var komsular = _ag.KomsuGetir(mevcutAdres).ToList();
                for (int i = komsular.Count - 1; i >= 0; i--)
                {
                    var hedef = komsular[i].Hedef;
                    if (!ziyaretEdilen.Contains(hedef))
                    {
                        kenarlar.Add((mevcutAdres, hedef));
                        yigit.Push((hedef, derinlik + 1));
                    }
                }
            }

            return new AnalizSonucu
            {
                ZiyaretEdilenDugumler = [.. ziyaretEdilen],
                ZiyaretEdilenKenarlar = kenarlar,
                ZiyaretSirasi = sira,
                Algoritma = "DFS",
                Baslangic = baslangic,
                MaxDerinlik = maxDerinlik,
            };
        }
    }

    // ============================================================
    // SONUÇ SINIFI
    // ============================================================

    /// <summary>BFS veya DFS analizinin sonucunu taşıyan veri nesnesi.</summary>
    public class AnalizSonucu
    {
        /// <summary>Ziyaret edilen tüm düğüm adresleri.</summary>
        public List<string> ZiyaretEdilenDugumler { get; set; } = [];

        /// <summary>Gezilen kenarlar: (kaynak adres, hedef adres) çiftleri.</summary>
        public List<(string Kaynak, string Hedef)> ZiyaretEdilenKenarlar { get; set; } = [];

        /// <summary>Düğümlerin ziyaret edildiği sıra.</summary>
        public List<string> ZiyaretSirasi { get; set; } = [];

        /// <summary>"BFS" veya "DFS".</summary>
        public string Algoritma { get; set; } = string.Empty;

        /// <summary>Başlangıç cüzdan adresi.</summary>
        public string Baslangic { get; set; } = string.Empty;

        /// <summary>Kullanılan maksimum derinlik.</summary>
        public int MaxDerinlik { get; set; }

        /// <summary>Boş (hata) sonucu oluşturur.</summary>
        public static AnalizSonucu Bos(string algoritma = "", string baslangic = "") =>
            new() { Algoritma = algoritma, Baslangic = baslangic };

        /// <summary>Sonucun özet metnini döndürür.</summary>
        public string OzetMetni()
        {
            if (ZiyaretSirasi.Count == 0)
                return "Sonuç bulunamadı.";

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"══ {Algoritma} Analiz Sonucu ══");
            sb.AppendLine($"Başlangıç cüzdan : {Baslangic}");
            sb.AppendLine($"Maksimum derinlik: {MaxDerinlik}");
            sb.AppendLine($"Ziyaret edilen   : {ZiyaretEdilenDugumler.Count} düğüm");
            sb.AppendLine($"Gezilen kenar    : {ZiyaretEdilenKenarlar.Count}");
            sb.AppendLine();
            sb.AppendLine($"─── {Algoritma} Geçiş Sırası ───");
            for (int i = 0; i < ZiyaretSirasi.Count; i++)
                sb.AppendLine($"  {i + 1,2}. {ZiyaretSirasi[i]}");
            return sb.ToString();
        }
    }
}