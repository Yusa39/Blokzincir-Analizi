using BlokzincirAnalizi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace deneme
{
    public partial class Form1 : Form
    {
        // ======================================================================
        // 🎨 FAZ 3 GÖRSELLEŞTİRME GLOBAL DEĞİŞKENLERİ
        // ======================================================================
        private BlokzincirAnalizi.BlokzincirAgi mevcutAg;
        private Dictionary<string, Point> dugumKonumlari = new Dictionary<string, Point>();
        private List<string> vurgulananYol = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Form yüklenirken yapılması gereken ek bir ayar varsa buraya gelebilir.
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // Metin değişim takibi alanı
        }

        // ======================================================================
        // 🧱 BUTON 1: AŞAMA 1 (MERKLE AĞACI VE BLOK TESTİ)
        // ======================================================================
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            // 1. Test için örnek işlemler oluşturuyoruz
            List<BlokzincirAnalizi.Islem> testIslemleri = new List<BlokzincirAnalizi.Islem>()
            {
                new BlokzincirAnalizi.Islem("Ahmet", "Mehmet", 100),
                new BlokzincirAnalizi.Islem("Mehmet", "Can", 50),
                new BlokzincirAnalizi.Islem("Can", "Aslı", 25)
            };

            richTextBox1.AppendText("🚀 1. Adım: Örnek işlemler oluşturuldu.\n");
            foreach (var islem in testIslemleri)
            {
                richTextBox1.AppendText($"   -> {islem.Gonderen} -> {islem.Alici}: {islem.Miktar} BTC (Hash: {islem.IslemHash})\n");
            }

            richTextBox1.AppendText("\n--------------------------------------------------\n\n");

            // 2. Merkle Ağacı sınıfını test ediyoruz
            richTextBox1.AppendText("🌳 2. Adım: Merkle Ağacı kökü hesaplanıyor...\n");
            string merkleRoot = BlokzincirAnalizi.MerkleAgaci.MerkleRootHesapla(testIslemleri);
            richTextBox1.AppendText($"   => Merkle Root: {merkleRoot}\n");

            richTextBox1.AppendText("\n--------------------------------------------------\n\n");

            // 3. Yeni bir blok (Düğüm) oluşturup zincire bağlama mantığını test ediyoruz
            richTextBox1.AppendText("🧱 3. Adım: Yeni blok (Düğüm) oluşturuluyor...\n");
            BlokzincirAnalizi.Dugum yeniBlok = new BlokzincirAnalizi.Dugum(1, testIslemleri, "00000000000000000000000000000000");

            richTextBox1.AppendText($"   => Blok Indeksi: {yeniBlok.Indeks}\n");
            richTextBox1.AppendText($"   => Blok Zamanı: {yeniBlok.Tarih}\n");
            richTextBox1.AppendText($"   => Bloğun Kendi Hash'i: {yeniBlok.Hash}\n");
        }

        // ======================================================================
        // ⚙️ BUTON 2: AŞAMA 2 VE 3 (SENTETİK VERİ ÜRETİMİ VE GRAF ÇİZİMİ)
        // ======================================================================
        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            vurgulananYol.Clear(); // Yeni üretimde eski vurguları sıfırla

            richTextBox1.AppendText("⚙️ Sentetik Graf Yapısı Üretiliyor...\n");
            richTextBox1.AppendText("--------------------------------------------------\n\n");

            // 8 cüzdan ve 12 işlem üretiyoruz
            mevcutAg = BlokzincirAnalizi.SentetikVeriUretici.Uret(cuzdanSayisi: 8, islemSayisi: 12, tohum: 42);

            richTextBox1.AppendText("👥 Graf Düğümleri ve Yönlü Kenarlar Hafızaya Yüklendi.\n");
            richTextBox1.AppendText($"📊 Toplam Ağ Hacmi: {mevcutAg.ToplamHacim()} BTC\n\n");
            richTextBox1.AppendText("💡 İPUCU: Grafı görmek için sağdaki panelGraf bileşenini izleyin.\n");
            richTextBox1.AppendText("💡 Fon analizi için aşağıdaki cüzdanlardan birini kutuya yazın:\n");

            foreach (var adres in mevcutAg.TumDugumler())
            {
                var cuzdan = mevcutAg.CuzdanGetir(adres);
                if (cuzdan != null)
                {
                    // 1. DÜZELTME HATA (image_692cc7.png): 'BaslangicBakiye' yerine sınıfındaki gerçek alan olan 'Bakiye' (veya Başlangıç mülkü) yazıldı.
                    richTextBox1.AppendText($"   🔹 Adres: {cuzdan.Adres} | Güncel Bakiye: {cuzdan.Bakiye} BTC\n");
                }
            }

            richTextBox1.AppendText("\n--------------------------------------------------\n");
            richTextBox1.AppendText("🔗 [GRAF KENARLARI] Cüzdanlar Arasındaki Fon Transfer Rotaları:\n");

            foreach (var islem in mevcutAg.Islemler)
            {
                richTextBox1.AppendText($"   {islem.Gonderen} ➡️ {islem.Alici} | Miktar: {islem.Miktar} BTC\n");
            }

            richTextBox1.AppendText("\n--------------------------------------------------\n");
            richTextBox1.AppendText($"📊 TOPLAM AĞ HACMİ: {mevcutAg.ToplamHacim()} BTC\n");

            // Grafik çizimini tetikliyoruz
            GrafiEkranaCiz(mevcutAg);
        }

        // ======================================================================
        // 🔍 ANALİZ BUTONU: BTNANALIZET (FON AKIŞ TAKİBİ VE VURGULAMA)
        // ======================================================================
        private void btnAnalizEt_Click_1(object sender, EventArgs e)
        {
            if (mevcutAg == null)
            {
                MessageBox.Show("Lütfen önce sentetik veriyi üretin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string girilenMetin = txtCuzdanId.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(girilenMetin))
            {
                MessageBox.Show("Lütfen analiz etmek için bir cüzdan adı girin (Örn: W01 veya W01_YFESGJ0S)!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 🌟 AKILLI EŞLEŞTİRME SİSTEMİ: 
            // Kullanıcı ister tam adı yazsın, ister ekranda gördüğü kısa adı (W01 gibi) yazsın, ağda bulur.
            string arananAdres = mevcutAg.TumDugumler()
                .FirstOrDefault(adres => adres.ToUpper() == girilenMetin || adres.Split('_')[0].ToUpper() == girilenMetin);

            if (arananAdres == null)
            {
                MessageBox.Show("Girilen cüzdan adresi ağda bulunamadı! Lütfen listeden tam adı kopyalayın veya ekrandaki kısa adı (Örn: W01) yazın.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            richTextBox1.Clear();
            richTextBox1.AppendText($"🔍 [{arananAdres}] Adresinden Başlayan Fon Akış Analizi:\n");
            richTextBox1.AppendText("--------------------------------------------------\n\n");

            vurgulananYol.Clear();

            List<string> ulasilanRota = new List<string>();

            try
            {
                // Komşuluk listesi üzerinden bu cüzdandan çıkan tüm transfer yollarını buluyoruz
                var komsular = mevcutAg.KomsuGetir(arananAdres);
                ulasilanRota.Add(arananAdres);
                foreach (var k in komsular)
                {
                    if (!ulasilanRota.Contains(k.Hedef))
                        ulasilanRota.Add(k.Hedef);
                }
            }
            catch
            {
                ulasilanRota = mevcutAg.TumDugumler().Where(d => d.StartsWith(arananAdres)).ToList();
            }

            if (ulasilanRota == null || ulasilanRota.Count <= 1)
            {
                vurgulananYol.Add(arananAdres);
                richTextBox1.AppendText("❌ Bu cüzdandan dışarıya doğru herhangi bir fon transfer akışı tespit edilemedi.\n");
            }
            else
            {
                richTextBox1.AppendText("🚀 Tespit Edilen Fon Rotaları:\n");
                foreach (var adres in ulasilanRota)
                {
                    string kısaAd = adres.Split('_')[0];
                    richTextBox1.AppendText($"   -> [HEDEF]: {kısaAd} ({adres})\n");
                    vurgulananYol.Add(adres);
                }

                richTextBox1.AppendText("\n🎨 GRAF GÜNCELLENDİ: Seçilen rotadaki cüzdanlar SARI, aralarındaki oklar KIRMIZI yapıldı!");
            }

            // Haritayı yeni renklerle yeniden çizdiriyoruz
            GrafiEkranaCiz(mevcutAg);
        }

        // ======================================================================
        // 🎨 SIFIRDAN YAZILAN GÖRSEL GRAF ÇİZİM MOTORU (GRAPH RENDERER)
        // ======================================================================
        private void GrafiEkranaCiz(BlokzincirAnalizi.BlokzincirAgi ag)
        {
            if (panelGraf == null) return;

            Graphics g = panelGraf.CreateGraphics();
            g.Clear(Color.White); // Tuvali temizle
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var dugumler = ag.TumDugumler().ToList();
            if (dugumler.Count == 0) return;

            int merkezX = panelGraf.Width / 2;
            int merkezY = panelGraf.Height / 2;
            int yaricap = Math.Min(merkezX, merkezY) - 60;

            dugumKonumlari.Clear();
            for (int i = 0; i < dugumler.Count; i++)
            {
                double aci = 2 * Math.PI * i / dugumler.Count;
                int x = merkezX + (int)(yaricap * Math.Cos(aci));
                int y = merkezY + (int)(yaricap * Math.Sin(aci));
                dugumKonumlari[dugumler[i]] = new Point(x, y);
            }

            foreach (var islem in ag.Islemler)
            {
                if (dugumKonumlari.ContainsKey(islem.Gonderen) && dugumKonumlari.ContainsKey(islem.Alici))
                {
                    Point pGonderen = dugumKonumlari[islem.Gonderen];
                    Point pAlici = dugumKonumlari[islem.Alici];

                    float cizgiKalinligi = Math.Max(1.5f, (float)islem.Miktar * 1.2f);
                    bool vurgula = vurgulananYol.Contains(islem.Gonderen) && vurgulananYol.Contains(islem.Alici);
                    Color cizgiRengi = vurgula ? Color.Red : Color.FromArgb(140, Color.LightSkyBlue);
                    if (vurgula) cizgiKalinligi += 2f;

                    using (Pen kalem = new Pen(cizgiRengi, cizgiKalinligi))
                    {
                        kalem.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(5, 5);
                        g.DrawLine(kalem, pGonderen, pAlici);
                    }
                }
            }

            foreach (var adres in dugumler)
            {
                var cuzdan = ag.CuzdanGetir(adres);
                Point konum = dugumKonumlari[adres];

                int daireBoyutu = 35 + (int)Math.Min(35, (cuzdan?.Bakiye ?? 0) / 3);
                int xOffset = konum.X - (daireBoyutu / 2);
                int yOffset = konum.Y - (daireBoyutu / 2);

                bool dugumVurgula = vurgulananYol.Contains(adres);
                Brush firca = dugumVurgula ? new SolidBrush(Color.Gold) : new SolidBrush(Color.DodgerBlue);
                Pen kenarlik = dugumVurgula ? new Pen(Color.OrangeRed, 3) : new Pen(Color.DarkBlue, 2);

                g.FillEllipse(firca, xOffset, yOffset, daireBoyutu, daireBoyutu);
                g.DrawEllipse(kenarlik, xOffset, yOffset, daireBoyutu, daireBoyutu);

                string kisaAd = adres.Split('_')[0];
                string bakiyeYazisi = $"{cuzdan?.Bakiye:F1} BTC";

                using (Font font = new Font("Arial", 9, FontStyle.Bold))
                {
                    g.DrawString(kisaAd, font, Brushes.Black, xOffset, yOffset - 18);
                    g.DrawString(bakiyeYazisi, font, Brushes.DarkGreen, xOffset, yOffset + daireBoyutu + 2);
                }

                firca.Dispose();
                kenarlik.Dispose();
            }
        }
    }
}