namespace deneme
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            richTextBox1 = new RichTextBox();
            button2 = new Button();
            panelGraf = new Panel();
            txtCuzdanId = new TextBox();
            btnAnalizEt = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(161, 29);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(42, 100);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(252, 348);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "";
            richTextBox1.TextChanged += richTextBox1_TextChanged;
            // 
            // button2
            // 
            button2.Location = new Point(203, 12);
            button2.Name = "button2";
            button2.Size = new Size(232, 29);
            button2.TabIndex = 2;
            button2.Text = "Sentetik Veri Üret ve Grafı Test Et";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // panelGraf
            // 
            panelGraf.Location = new Point(615, 41);
            panelGraf.Name = "panelGraf";
            panelGraf.Size = new Size(446, 424);
            panelGraf.TabIndex = 3;
            // 
            // txtCuzdanId
            // 
            txtCuzdanId.Location = new Point(1087, 60);
            txtCuzdanId.Name = "txtCuzdanId";
            txtCuzdanId.Size = new Size(220, 27);
            txtCuzdanId.TabIndex = 4;
            // 
            // btnAnalizEt
            // 
            btnAnalizEt.Location = new Point(1126, 166);
            btnAnalizEt.Name = "btnAnalizEt";
            btnAnalizEt.Size = new Size(193, 153);
            btnAnalizEt.TabIndex = 5;
            btnAnalizEt.Text = "Fon Akışını Analiz Et (BFS/DFS)";
            btnAnalizEt.UseVisualStyleBackColor = true;
            btnAnalizEt.Click += btnAnalizEt_Click_1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1319, 502);
            Controls.Add(btnAnalizEt);
            Controls.Add(txtCuzdanId);
            Controls.Add(panelGraf);
            Controls.Add(button2);
            Controls.Add(richTextBox1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private RichTextBox richTextBox1;
        private Button button2;
        private Panel panelGraf;
        private TextBox txtCuzdanId;
        private Button btnAnalizEt;
    }
}
