using System.Drawing.Drawing2D;

namespace BlokzincirAnalizi
{
    /// <summary>
    /// Ana form — Streamlit arayüzünün Windows Forms karşılığı.
    ///
    /// İşlevler:
    ///   - Sentetik veri üretme ve tablo güncelleme
    ///   - GDI+ ile interaktif graf çizimi (PictureBox üzerinde)
    ///   - BFS/DFS analizi çalıştırma ve sonuç vurgulama
    ///   - Merkle ağacı bilgisi gösterme
    /// </summary>
    public partial class Form1 : Form
    {
        // ── İş mantığı nesneleri ───────────────────────────────────────
        private BlokzincirAgi      _ag       = new();
        private MerkleAgaci?       _merkle;
        private FonAkisiAnalizoru? _analizor;
        private AnalizSonucu?      _sonuc;

        // ── Graf çizim sabitleri ───────────────────────────────────────
        private const float DugumYaricap = 22f;
        private const float OkUzunluk   = 11f;
        private const float OkGenislik  = 5f;

        // Düğüm merkez koordinatları (adres → PointF)
        private Dictionary<string, PointF> _pozisyonlar = [];

        // ── Renk paleti (graf için) ────────────────────────────────────
        private static readonly Color RenkArka         = Color.FromArgb(14, 17, 23);
        private static readonly Color RenkDugumNormal  = Color.FromArgb(28, 75, 155);
        private static readonly Color RenkDugumKenar   = Color.FromArgb(74, 144, 217);
        private static readonly Color RenkDugumVurgu   = Color.FromArgb(220, 90, 35);
        private static readonly Color RenkDugumVurguK  = Color.FromArgb(255, 200, 0);
        private static readonly Color RenkBaslangic    = Color.FromArgb(50, 185, 100);
        private static readonly Color RenkKenarNormal  = Color.FromArgb(70, 85, 110);
        private static readonly Color RenkKenarVurgu   = Color.OrangeRed;

        // ── Constructor ───────────────────────────────────────────────

        public Form1()
        {
            InitializeComponent();

            // Olay bağlantıları
            btnVeriUret.Click     += (_, _) => VeriUret();
            btnAnalizBaslat.Click += (_, _) => AnalizBaslat();
            btnTemizle.Click      += (_, _) => VurgulamaTemizle();
            btnMerkleDetay.Click  += (_, _) => MerkleDetayGoster();
            picGraf.Paint         += PicGraf_Paint;
            picGraf.Resize        += (_, _) => { HesaplaPozisyonlar(); picGraf.Invalidate(); };

            // Başlangıç verisi yükle
            VeriUret(tohum: 42);
        }

        // ══════════════════════════════════════════════════════════════
        // VERİ YÖNETİMİ
        // ══════════════════════════════════════════════════════════════

        /// <summary>Sentetik ağ üretir ve tüm UI bileşenlerini günceller.</summary>
        private void VeriUret(int tohum = -1)
        {
            if (tohum < 0) tohum = Random.Shared.Next(99999);

            _ag       = SentetikVeriUretici.Uret(cuzdanSayisi: 18, islemSayisi: 45, tohum: tohum);
            _merkle   = new MerkleAgaci(_ag.Islemler.ToList());
            _analizor = new FonAkisiAnalizoru(_ag);
            _sonuc    = null;

            CuzdanComboGuncelle();
            CuzdanTablosuGuncelle();
            IslemTablosuGuncelle();
            IstatistikGuncelle();
            MerkleEtiketGuncelle();
            HesaplaPozisyonlar();

            picGraf.Invalidate();
            rtbSonuc.Text = "Bir cüzdan seçip analizi başlatın...";
        }

        // ══════════════════════════════════════════════════════════════
        // BFS / DFS ANALİZ
        // ══════════════════════════════════════════════════════════════

        /// <summary>Seçili cüzdan ve algoritmaya göre BFS veya DFS çalıştırır.</summary>
        private void AnalizBaslat()
        {
            if (_analizor == null || cboCuzdan.SelectedItem is not string adres || string.IsNullOrEmpty(adres))
            {
                MessageBox.Show("Lütfen bir cüzdan seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int derinlik = (int)nudDerinlik.Value;

            _sonuc = rbBFS.Checked
                ? _analizor.BfsFonAkisi(adres, derinlik)
                : _analizor.DfsFonAkisi(adres, derinlik);

            // Graf'ı vurgularla yeniden çiz
            picGraf.Invalidate();

            // Sonuç sekmesini güncelle ve öne getir
            rtbSonuc.Text = _sonuc.OzetMetni();
            tabMain.SelectedTab = tabSonuc;
        }

        /// <summary>Graf vurgusunu temizler.</summary>
        private void VurgulamaTemizle()
        {
            _sonuc = null;
            picGraf.Invalidate();
            rtbSonuc.Text = "Vurgulama temizlendi.";
        }

        // ══════════════════════════════════════════════════════════════
        // GRAF ÇİZİMİ (GDI+)
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Düğümlerin PictureBox üzerindeki piksel koordinatlarını hesaplar.
        /// Dairesel yerleşim algoritması: tüm düğümleri eşit açılarla bir çember üzerine dağıtır.
        /// </summary>
        private void HesaplaPozisyonlar()
        {
            _pozisyonlar.Clear();
            var dugumler = _ag.TumDugumler().ToList();
            int n = dugumler.Count;
            if (n == 0) return;

            float w  = picGraf.ClientSize.Width;
            float h  = picGraf.ClientSize.Height;
            float cx = w / 2f;
            float cy = h / 2f;
            float r  = Math.Min(w, h) / 2f - 55f;

            for (int i = 0; i < n; i++)
            {
                // Üstten başla (−π/2) ve saat yönünde ilerle
                double aci = 2 * Math.PI * i / n - Math.PI / 2;
                float x = cx + r * (float)Math.Cos(aci);
                float y = cy + r * (float)Math.Sin(aci);
                _pozisyonlar[dugumler[i]] = new PointF(x, y);
            }
        }

        /// <summary>PictureBox.Paint olayı — her yeniden boyama isteğinde çağrılır.</summary>
        private void PicGraf_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode      = SmoothingMode.AntiAlias;
            g.TextRenderingHint  = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(RenkArka);

            if (_ag.Cuzdanlar.Count == 0 || _pozisyonlar.Count == 0)
            {
                using var fnt = new Font("Segoe UI", 12f);
                g.DrawString("Veri yükleniyor...", fnt, Brushes.Gray,
                    picGraf.Width / 2f - 70, picGraf.Height / 2f);
                return;
            }

            // Vurgulanan kenar ve düğüm setleri (hızlı arama için HashSet)
            var vurguKenarlar = _sonuc?.ZiyaretEdilenKenarlar
                .Select(k => (k.Kaynak, k.Hedef))
                .ToHashSet() ?? [];
            var vurguDugumler = _sonuc?.ZiyaretEdilenDugumler.ToHashSet() ?? [];

            // Bakiye değerlerini normalize et (düğüm boyutu için)
            var bakiyeler = _ag.Cuzdanlar.Values.Select(c => (double)c.Bakiye).ToList();
            double minB = bakiyeler.Min(), maxB = bakiyeler.Max();
            double spanB = maxB - minB < 0.001 ? 1 : maxB - minB;

            // ── 1. Kenarları çiz (düğümlerin altında kalsın) ──────────
            foreach (var kaynak in _ag.TumDugumler())
            {
                if (!_pozisyonlar.TryGetValue(kaynak, out var kaynakPos)) continue;

                foreach (var kenar in _ag.KomsuGetir(kaynak))
                {
                    if (!_pozisyonlar.TryGetValue(kenar.Hedef, out var hedefPos)) continue;
                    if (kaynak == kenar.Hedef) continue; // öz-döngü atla

                    bool vurgulanan = vurguKenarlar.Contains((kaynak, kenar.Hedef));
                    float kalinlik  = vurgulanan ? 2.8f : 1.1f;
                    Color renk      = vurgulanan ? RenkKenarVurgu : RenkKenarNormal;

                    using var kalem = new Pen(renk, kalinlik);
                    OkluCizgiCiz(g, kalem, kaynakPos, hedefPos, DugumYaricap);
                }
            }

            // ── 2. Düğümleri çiz (kenarların üstünde) ─────────────────
            using var etiketFont = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            using var bakiyeFont = new Font("Segoe UI", 6.5f);

            foreach (var adres in _ag.TumDugumler())
            {
                if (!_pozisyonlar.TryGetValue(adres, out var pos)) continue;

                var cuzdan = _ag.CuzdanGetir(adres)!;
                bool baslangic  = adres == _sonuc?.Baslangic;
                bool vurgulanan = vurguDugumler.Contains(adres);

                // Bakiyeye orantılı yarıçap: 16–32 px
                double norm = (double)(cuzdan.Bakiye - (decimal)minB) / spanB;
                float  yar  = (float)(16 + norm * 16);

                Color dolgu  = baslangic  ? RenkBaslangic
                             : vurgulanan ? RenkDugumVurgu
                                          : RenkDugumNormal;
                Color kenar  = baslangic  ? Color.LimeGreen
                             : vurgulanan ? RenkDugumVurguK
                                          : RenkDugumKenar;
                float kKalin = (baslangic || vurgulanan) ? 2.5f : 1.5f;

                var rect = new RectangleF(pos.X - yar, pos.Y - yar, yar * 2, yar * 2);

                // Dolgu
                using (var br = new SolidBrush(dolgu))
                    g.FillEllipse(br, rect);

                // Kenarlık
                using (var kn = new Pen(kenar, kKalin))
                    g.DrawEllipse(kn, rect);

                // Adres etiketi
                string kisaAdres = adres.Length > 9 ? adres[..9] : adres;
                var sz = g.MeasureString(kisaAdres, etiketFont);
                g.DrawString(kisaAdres, etiketFont, Brushes.White,
                    pos.X - sz.Width / 2f, pos.Y - sz.Height / 2f - 3f);

                // Bakiye etiketi
                string bakiyeStr = $"{cuzdan.Bakiye:F2}₿";
                var szB = g.MeasureString(bakiyeStr, bakiyeFont);
                using var bakiyeFirca = new SolidBrush(Color.FromArgb(190, 220, 255));
                g.DrawString(bakiyeStr, bakiyeFont, bakiyeFirca,
                    pos.X - szB.Width / 2f, pos.Y + sz.Height / 2f - 1f);
            }

            // ── 3. Başlık / Açıklama ──────────────────────────────────
            if (_sonuc != null)
            {
                string aciklama = $"{_sonuc.Algoritma} | Başlangıç: {_sonuc.Baslangic} "
                                + $"| {_sonuc.ZiyaretEdilenDugumler.Count} düğüm "
                                + $"| {_sonuc.ZiyaretEdilenKenarlar.Count} kenar";
                using var acFont = new Font("Segoe UI", 9f);
                using var acBr   = new SolidBrush(Color.FromArgb(220, 170, 60));
                g.DrawString(aciklama, acFont, acBr, 8f, 8f);
            }

            // ── 4. Renk Açıklaması ────────────────────────────────────
            CizAciklama(g, picGraf.ClientSize);
        }

        /// <summary>
        /// İki çember arasında oklu çizgi (kenar) çizer.
        /// Çember merkezleri yerine çember kenarlarından başlar/biter.
        /// </summary>
        private static void OkluCizgiCiz(Graphics g, Pen kalem,
            PointF kaynak, PointF hedef, float yaricap)
        {
            float dx  = hedef.X - kaynak.X;
            float dy  = hedef.Y - kaynak.Y;
            float uz  = (float)Math.Sqrt(dx * dx + dy * dy);
            if (uz < 1f) return;

            float nx = dx / uz, ny = dy / uz;

            // Çember kenarından başla ve bitir
            PointF bas  = new(kaynak.X + nx * yaricap,  kaynak.Y + ny * yaricap);
            PointF bitis= new(hedef.X  - nx * yaricap,  hedef.Y  - ny * yaricap);

            g.DrawLine(kalem, bas, bitis);

            // Ok başı üçgeni
            PointF p1 = new(bitis.X - OkUzunluk * nx + OkGenislik * ny,
                            bitis.Y - OkUzunluk * ny - OkGenislik * nx);
            PointF p2 = new(bitis.X - OkUzunluk * nx - OkGenislik * ny,
                            bitis.Y - OkUzunluk * ny + OkGenislik * nx);

            using var firca = new SolidBrush(kalem.Color);
            g.FillPolygon(firca, [bitis, p1, p2]);
        }

        /// <summary>Sağ alt köşeye renk açıklaması çizer.</summary>
        private static void CizAciklama(Graphics g, Size boyut)
        {
            var satirlar = new (Color Renk, string Metin)[]
            {
                (Color.LimeGreen,      "● Başlangıç cüzdanı"),
                (Color.OrangeRed,      "● BFS/DFS yolu"),
                (Color.FromArgb(74,144,217), "● Normal cüzdan"),
                (Color.OrangeRed,      "→ İzlenen kenar"),
            };

            using var fnt = new Font("Segoe UI", 8f);
            int x = boyut.Width - 170, y = boyut.Height - 12 - satirlar.Length * 17;

            foreach (var (renk, metin) in satirlar)
            {
                using var br = new SolidBrush(renk);
                g.DrawString(metin, fnt, br, x, y);
                y += 17;
            }
        }

        // ══════════════════════════════════════════════════════════════
        // TABLO GÜNCELLEMELERİ
        // ══════════════════════════════════════════════════════════════

        private void CuzdanComboGuncelle()
        {
            cboCuzdan.Items.Clear();
            foreach (var adres in _ag.Cuzdanlar.Keys.OrderBy(a => a))
                cboCuzdan.Items.Add(adres);
            if (cboCuzdan.Items.Count > 0)
                cboCuzdan.SelectedIndex = 0;
        }

        private void CuzdanTablosuGuncelle()
        {
            dgvCuzdanlar.DataSource = null;
            dgvCuzdanlar.Columns.Clear();

            var tablo = _ag.Cuzdanlar.Values
                .OrderByDescending(c => c.Bakiye)
                .Select(c => new
                {
                    Adres           = c.Adres,
                    Bakiye_BTC      = c.Bakiye,
                    Gelen_İşlem     = c.GelenIslemler.Count,
                    Giden_İşlem     = c.GidenIslemler.Count,
                    Toplam_Gelen    = c.ToplamGelen,
                    Toplam_Giden    = c.ToplamGiden,
                })
                .ToList();

            dgvCuzdanlar.DataSource = tablo;

            // Sütun genişlikleri
            if (dgvCuzdanlar.Columns.Count > 0)
            {
                dgvCuzdanlar.Columns[0].FillWeight = 35;
                dgvCuzdanlar.Columns[1].FillWeight = 15;
                for (int i = 2; i < dgvCuzdanlar.Columns.Count; i++)
                    dgvCuzdanlar.Columns[i].FillWeight = 12;
            }
        }

        private void IslemTablosuGuncelle()
        {
            dgvIslemler.DataSource = null;
            dgvIslemler.Columns.Clear();

            var tablo = _ag.Islemler
                .OrderByDescending(i => i.Zaman)
                .Select(i => new
                {
                    TxID       = i.TxId,
                    Gönderici  = i.Gonderen,
                    Alıcı      = i.Alici,
                    Miktar_BTC = i.Miktar,
                    Zaman      = i.Zaman.ToString("yyyy-MM-dd HH:mm"),
                })
                .ToList();

            dgvIslemler.DataSource = tablo;
        }

        private void IstatistikGuncelle()
        {
            lblIstatistik.Text =
                $"💼 {_ag.Cuzdanlar.Count} cüzdan  " +
                $"📋 {_ag.Islemler.Count} işlem  " +
                $"🔗 {_ag.ToplamKenarSayisi()} kenar";
        }

        private void MerkleEtiketGuncelle()
        {
            if (_merkle == null) return;
            lblMerkleVal.Text = _merkle.Kok[..24] + "...";
        }

        // ══════════════════════════════════════════════════════════════
        // MERKLE DETAY
        // ══════════════════════════════════════════════════════════════

        private void MerkleDetayGoster()
        {
            if (_merkle == null) return;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("══════════════════════════════════════════");
            sb.AppendLine("           MERKLE AĞACI DETAYI");
            sb.AppendLine("══════════════════════════════════════════");
            sb.AppendLine($"İşlem Sayısı : {_merkle.IslemSayisi}");
            sb.AppendLine($"Ağaç Derinliği: {_merkle.Derinlik}");
            sb.AppendLine();
            sb.AppendLine($"MERKLE ROOT:");
            sb.AppendLine($"  {_merkle.Kok}");
            sb.AppendLine();

            for (int i = 0; i < _merkle.Seviyeler.Count; i++)
            {
                var seviye = _merkle.Seviyeler[i];
                string ad = i == 0 ? "Yapraklar (Leaf)"
                          : i == _merkle.Seviyeler.Count - 1 ? "Kök (Root)"
                          : $"Seviye {i}";

                sb.AppendLine($"─── {ad} ({seviye.Count} hash) ───");
                foreach (var h in seviye.Take(4))
                    sb.AppendLine($"  {h}");
                if (seviye.Count > 4)
                    sb.AppendLine($"  ... ve {seviye.Count - 4} hash daha");
                sb.AppendLine();
            }

            // Sonuç sekmesine yaz ve öne getir
            rtbSonuc.Text = sb.ToString();
            tabMain.SelectedTab = tabSonuc;
        }
    }
}
