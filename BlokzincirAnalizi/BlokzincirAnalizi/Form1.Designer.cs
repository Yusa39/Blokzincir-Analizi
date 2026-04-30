namespace BlokzincirAnalizi
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            // ── Kontrol tanımları ──────────────────────────────────────
            pnlSol          = new Panel();
            lblBaslik       = new Label();
            grpVeri         = new GroupBox();
            btnVeriUret     = new Button();
            lblIstatistik   = new Label();
            grpAnaliz       = new GroupBox();
            lblCuzdanSec    = new Label();
            cboCuzdan       = new ComboBox();
            lblAlgoritma    = new Label();
            rbBFS           = new RadioButton();
            rbDFS           = new RadioButton();
            lblDerinlik     = new Label();
            nudDerinlik     = new NumericUpDown();
            btnAnalizBaslat = new Button();
            btnTemizle      = new Button();
            grpMerkle       = new GroupBox();
            lblMerkleBaslik = new Label();
            lblMerkleVal    = new Label();
            btnMerkleDetay  = new Button();
            tabMain         = new TabControl();
            tabGraf         = new TabPage();
            picGraf         = new PictureBox();
            tabCuzdanlar    = new TabPage();
            dgvCuzdanlar    = new DataGridView();
            tabIslemler     = new TabPage();
            dgvIslemler     = new DataGridView();
            tabSonuc        = new TabPage();
            rtbSonuc        = new RichTextBox();

            // ── Renk sabitleri ─────────────────────────────────────────
            var renkArka     = Color.FromArgb(14,  17,  23);
            var renkPanel    = Color.FromArgb(22,  27,  42);
            var renkKoyu     = Color.FromArgb(30,  37,  58);
            var renkMavi     = Color.FromArgb(74, 144, 217);
            var renkBeyaz    = Color.White;
            var renkGri      = Color.FromArgb(160, 165, 175);
            var renkDugme    = Color.FromArgb(28,  80, 170);
            var renkYesil    = Color.FromArgb(50, 180, 120);

            // ──────────────────────────────────────────────────────────
            // SOL PANEL
            // ──────────────────────────────────────────────────────────
            pnlSol.Dock      = DockStyle.Left;
            pnlSol.Width     = 268;
            pnlSol.BackColor = renkPanel;
            pnlSol.Padding   = new Padding(8);

            // Başlık
            lblBaslik.Text      = "⛓️ Blokzincir Analizi";
            lblBaslik.Font      = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblBaslik.ForeColor = renkMavi;
            lblBaslik.Dock      = DockStyle.Top;
            lblBaslik.Height    = 38;
            lblBaslik.TextAlign = ContentAlignment.MiddleCenter;
            lblBaslik.BackColor = renkKoyu;
            lblBaslik.Margin    = new Padding(0, 0, 0, 6);

            // ── GroupBox: Veri ─────────────────────────────────────────
            grpVeri.Text      = " 📦 Veri ";
            grpVeri.ForeColor = renkGri;
            grpVeri.BackColor = renkPanel;
            grpVeri.Dock      = DockStyle.Top;
            grpVeri.Height    = 88;
            grpVeri.Padding   = new Padding(8, 4, 8, 4);

            StilDugme(btnVeriUret, "🔄  Yeni Veri Üret", renkYesil);
            btnVeriUret.Location = new Point(8, 22);
            btnVeriUret.Width    = 234;

            lblIstatistik.Text      = "—";
            lblIstatistik.ForeColor = renkGri;
            lblIstatistik.Font      = new Font("Segoe UI", 8f);
            lblIstatistik.Location  = new Point(8, 58);
            lblIstatistik.Size      = new Size(234, 22);

            grpVeri.Controls.Add(btnVeriUret);
            grpVeri.Controls.Add(lblIstatistik);

            // ── GroupBox: Fon Akışı Analizi ───────────────────────────
            grpAnaliz.Text      = " 🔍 Fon Akışı Analizi ";
            grpAnaliz.ForeColor = renkGri;
            grpAnaliz.BackColor = renkPanel;
            grpAnaliz.Dock      = DockStyle.Top;
            grpAnaliz.Height    = 268;
            grpAnaliz.Padding   = new Padding(8, 4, 8, 4);

            lblCuzdanSec.Text      = "Başlangıç Cüzdanı:";
            lblCuzdanSec.ForeColor = renkGri;
            lblCuzdanSec.Font      = new Font("Segoe UI", 8.5f);
            lblCuzdanSec.Location  = new Point(8, 22);
            lblCuzdanSec.AutoSize  = true;

            cboCuzdan.Location     = new Point(8, 40);
            cboCuzdan.Width        = 234;
            cboCuzdan.DropDownStyle= ComboBoxStyle.DropDownList;
            cboCuzdan.BackColor    = renkKoyu;
            cboCuzdan.ForeColor    = renkBeyaz;
            cboCuzdan.FlatStyle    = FlatStyle.Flat;
            cboCuzdan.Font         = new Font("Segoe UI", 8.5f);

            lblAlgoritma.Text      = "Algoritma:";
            lblAlgoritma.ForeColor = renkGri;
            lblAlgoritma.Font      = new Font("Segoe UI", 8.5f);
            lblAlgoritma.Location  = new Point(8, 74);
            lblAlgoritma.AutoSize  = true;

            StilRadyo(rbBFS, "BFS  (Genişlik Öncelikli — Queue)", renkBeyaz);
            rbBFS.Location = new Point(8, 92);
            rbBFS.Checked  = true;

            StilRadyo(rbDFS, "DFS  (Derinlik Öncelikli — Stack)", renkBeyaz);
            rbDFS.Location = new Point(8, 114);

            lblDerinlik.Text      = "Maksimum Derinlik:";
            lblDerinlik.ForeColor = renkGri;
            lblDerinlik.Font      = new Font("Segoe UI", 8.5f);
            lblDerinlik.Location  = new Point(8, 140);
            lblDerinlik.AutoSize  = true;

            nudDerinlik.Location  = new Point(8, 158);
            nudDerinlik.Width     = 80;
            nudDerinlik.Minimum   = 1;
            nudDerinlik.Maximum   = 8;
            nudDerinlik.Value     = 3;
            nudDerinlik.BackColor = renkKoyu;
            nudDerinlik.ForeColor = renkBeyaz;

            StilDugme(btnAnalizBaslat, "🚀  Analizi Başlat", renkDugme);
            btnAnalizBaslat.Location = new Point(8, 190);
            btnAnalizBaslat.Width    = 234;

            StilDugme(btnTemizle, "❌  Vurgulamayı Temizle", Color.FromArgb(120, 30, 30));
            btnTemizle.Location = new Point(8, 226);
            btnTemizle.Width    = 234;

            grpAnaliz.Controls.AddRange([
                lblCuzdanSec, cboCuzdan, lblAlgoritma,
                rbBFS, rbDFS, lblDerinlik, nudDerinlik,
                btnAnalizBaslat, btnTemizle
            ]);

            // ── GroupBox: Merkle ──────────────────────────────────────
            grpMerkle.Text      = " 🌳 Merkle Ağacı ";
            grpMerkle.ForeColor = renkGri;
            grpMerkle.BackColor = renkPanel;
            grpMerkle.Dock      = DockStyle.Top;
            grpMerkle.Height    = 112;
            grpMerkle.Padding   = new Padding(8, 4, 8, 4);

            lblMerkleBaslik.Text      = "Merkle Root:";
            lblMerkleBaslik.ForeColor = renkGri;
            lblMerkleBaslik.Font      = new Font("Segoe UI", 8.5f);
            lblMerkleBaslik.Location  = new Point(8, 22);
            lblMerkleBaslik.AutoSize  = true;

            lblMerkleVal.Text      = "—";
            lblMerkleVal.ForeColor = Color.FromArgb(100, 220, 130);
            lblMerkleVal.Font      = new Font("Consolas", 8f);
            lblMerkleVal.Location  = new Point(8, 40);
            lblMerkleVal.Size      = new Size(234, 28);
            lblMerkleVal.AutoEllipsis = true;

            StilDugme(btnMerkleDetay, "🔎  Detayları Göster", Color.FromArgb(40, 100, 60));
            btnMerkleDetay.Location = new Point(8, 74);
            btnMerkleDetay.Width    = 234;

            grpMerkle.Controls.AddRange([lblMerkleBaslik, lblMerkleVal, btnMerkleDetay]);

            // Sol panele ekle (ters sırada — en son eklenen en üstte)
            pnlSol.Controls.Add(grpMerkle);
            pnlSol.Controls.Add(grpAnaliz);
            pnlSol.Controls.Add(grpVeri);
            pnlSol.Controls.Add(lblBaslik);

            // ──────────────────────────────────────────────────────────
            // SAĞ TARAF — TAB CONTROL
            // ──────────────────────────────────────────────────────────
            tabMain.Dock      = DockStyle.Fill;
            tabMain.BackColor = renkArka;
            tabMain.Font      = new Font("Segoe UI", 9.5f);

            // Tab 1: Graf
            tabGraf.Text      = "  🌐 Graf  ";
            tabGraf.BackColor = renkArka;

            picGraf.Dock      = DockStyle.Fill;
            picGraf.BackColor = renkArka;
            picGraf.SizeMode  = PictureBoxSizeMode.Normal;
            picGraf.Cursor    = Cursors.Cross;

            tabGraf.Controls.Add(picGraf);
            tabMain.TabPages.Add(tabGraf);

            // Tab 2: Cüzdanlar
            tabCuzdanlar.Text      = "  💰 Cüzdanlar  ";
            tabCuzdanlar.BackColor = renkArka;

            StilGrid(dgvCuzdanlar, renkArka, renkBeyaz, renkKoyu, renkMavi);
            dgvCuzdanlar.Dock = DockStyle.Fill;

            tabCuzdanlar.Controls.Add(dgvCuzdanlar);
            tabMain.TabPages.Add(tabCuzdanlar);

            // Tab 3: İşlemler
            tabIslemler.Text      = "  📜 İşlemler  ";
            tabIslemler.BackColor = renkArka;

            StilGrid(dgvIslemler, renkArka, renkBeyaz, renkKoyu, renkMavi);
            dgvIslemler.Dock = DockStyle.Fill;

            tabIslemler.Controls.Add(dgvIslemler);
            tabMain.TabPages.Add(tabIslemler);

            // Tab 4: BFS/DFS Sonucu
            tabSonuc.Text      = "  📊 BFS/DFS Sonucu  ";
            tabSonuc.BackColor = renkArka;

            rtbSonuc.Dock      = DockStyle.Fill;
            rtbSonuc.BackColor = renkArka;
            rtbSonuc.ForeColor = Color.FromArgb(180, 220, 255);
            rtbSonuc.Font      = new Font("Consolas", 9.5f);
            rtbSonuc.ReadOnly  = true;
            rtbSonuc.Text      = "Bir cüzdan seçip analizi başlatın...";

            tabSonuc.Controls.Add(rtbSonuc);
            tabMain.TabPages.Add(tabSonuc);

            // ──────────────────────────────────────────────────────────
            // FORM
            // ──────────────────────────────────────────────────────────
            this.SuspendLayout();
            this.Text            = "Blokzincir İşlem Ağları Analizi — Veri Yapıları Projesi";
            this.ClientSize      = new Size(1160, 720);
            this.MinimumSize     = new Size(900, 600);
            this.BackColor       = renkArka;
            this.Font            = new Font("Segoe UI", 9f);
            this.StartPosition   = FormStartPosition.CenterScreen;

            this.Controls.Add(tabMain);
            this.Controls.Add(pnlSol);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // ── Yardımcı Stil Metotları ────────────────────────────────────

        private static void StilDugme(Button btn, string metin, Color arkaPlan)
        {
            btn.Text       = metin;
            btn.Height     = 30;
            btn.FlatStyle  = FlatStyle.Flat;
            btn.BackColor  = arkaPlan;
            btn.ForeColor  = Color.White;
            btn.Font       = new Font("Segoe UI", 9f, FontStyle.Bold);
            btn.Cursor     = Cursors.Hand;
            // 0-255 sınırını aşmamak için kıstır
            btn.FlatAppearance.BorderColor = Color.FromArgb(
                Math.Min(255, arkaPlan.R + 30),
                Math.Min(255, arkaPlan.G + 30),
                Math.Min(255, arkaPlan.B + 30));
        }

        private static void StilRadyo(RadioButton rb, string metin, Color onPlan)
        {
            rb.Text      = metin;
            rb.ForeColor = onPlan;
            rb.BackColor = Color.Transparent;
            rb.Font      = new Font("Segoe UI", 8.5f);
            rb.AutoSize  = true;
        }

        private static void StilGrid(DataGridView dgv,
            Color arka, Color on, Color baslik, Color secim)
        {
            dgv.BackgroundColor                        = arka;
            dgv.ForeColor                              = on;
            dgv.GridColor                              = Color.FromArgb(45, 55, 80);
            dgv.BorderStyle                            = BorderStyle.None;
            dgv.RowHeadersVisible                      = false;
            dgv.AllowUserToAddRows                     = false;
            dgv.AllowUserToDeleteRows                  = false;
            dgv.ReadOnly                               = true;
            dgv.SelectionMode                          = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode                    = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.EnableHeadersVisualStyles              = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor= baslik;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor= on;
            dgv.ColumnHeadersDefaultCellStyle.Font     = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgv.ColumnHeadersBorderStyle               = DataGridViewHeaderBorderStyle.Single;
            dgv.DefaultCellStyle.BackColor             = arka;
            dgv.DefaultCellStyle.ForeColor             = on;
            dgv.DefaultCellStyle.SelectionBackColor    = secim;
            dgv.DefaultCellStyle.SelectionForeColor    = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 30, 48);
            dgv.Font                                   = new Font("Segoe UI", 9f);
        }

        #endregion

        // ── Kontrol değişkenleri ──────────────────────────────────────
        private Panel          pnlSol;
        private Label          lblBaslik;
        private GroupBox       grpVeri;
        private Button         btnVeriUret;
        private Label          lblIstatistik;
        private GroupBox       grpAnaliz;
        private Label          lblCuzdanSec;
        private ComboBox       cboCuzdan;
        private Label          lblAlgoritma;
        private RadioButton    rbBFS;
        private RadioButton    rbDFS;
        private Label          lblDerinlik;
        private NumericUpDown  nudDerinlik;
        private Button         btnAnalizBaslat;
        private Button         btnTemizle;
        private GroupBox       grpMerkle;
        private Label          lblMerkleBaslik;
        private Label          lblMerkleVal;
        private Button         btnMerkleDetay;
        private TabControl     tabMain;
        private TabPage        tabGraf;
        private PictureBox     picGraf;
        private TabPage        tabCuzdanlar;
        private DataGridView   dgvCuzdanlar;
        private TabPage        tabIslemler;
        private DataGridView   dgvIslemler;
        private TabPage        tabSonuc;
        private RichTextBox    rtbSonuc;
    }
}
