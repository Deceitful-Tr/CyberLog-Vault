namespace CyberLog_Vault
{
    partial class Form1
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnGiris = new MaterialSkin.Controls.MaterialButton();
            this.txtKullaniciAdi = new MaterialSkin.Controls.MaterialTextBox();
            this.txtSifre = new MaterialSkin.Controls.MaterialTextBox2();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGiris
            // 
            this.btnGiris.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnGiris.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnGiris.Depth = 0;
            this.btnGiris.HighEmphasis = true;
            this.btnGiris.Icon = null;
            this.btnGiris.Location = new System.Drawing.Point(308, 383);
            this.btnGiris.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnGiris.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnGiris.Name = "btnGiris";
            this.btnGiris.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnGiris.Size = new System.Drawing.Size(154, 36);
            this.btnGiris.TabIndex = 7;
            this.btnGiris.Text = "SİSTEME GİRİŞ YAP";
            this.btnGiris.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnGiris.UseAccentColor = false;
            this.btnGiris.UseVisualStyleBackColor = true;
            this.btnGiris.Click += new System.EventHandler(this.btnGiris_Click);
            // 
            // txtKullaniciAdi
            // 
            this.txtKullaniciAdi.AnimateReadOnly = false;
            this.txtKullaniciAdi.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtKullaniciAdi.Depth = 0;
            this.txtKullaniciAdi.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtKullaniciAdi.Hint = "Kullanıcı Adı";
            this.txtKullaniciAdi.LeadingIcon = null;
            this.txtKullaniciAdi.Location = new System.Drawing.Point(276, 244);
            this.txtKullaniciAdi.MaxLength = 50;
            this.txtKullaniciAdi.MouseState = MaterialSkin.MouseState.OUT;
            this.txtKullaniciAdi.Multiline = false;
            this.txtKullaniciAdi.Name = "txtKullaniciAdi";
            this.txtKullaniciAdi.Size = new System.Drawing.Size(228, 50);
            this.txtKullaniciAdi.TabIndex = 8;
            this.txtKullaniciAdi.Text = "";
            this.txtKullaniciAdi.TrailingIcon = null;
            // 
            // txtSifre
            // 
            this.txtSifre.AnimateReadOnly = false;
            this.txtSifre.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtSifre.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtSifre.Depth = 0;
            this.txtSifre.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtSifre.HideSelection = true;
            this.txtSifre.Hint = "Şifre";
            this.txtSifre.LeadingIcon = null;
            this.txtSifre.Location = new System.Drawing.Point(276, 326);
            this.txtSifre.MaxLength = 32767;
            this.txtSifre.MouseState = MaterialSkin.MouseState.OUT;
            this.txtSifre.Name = "txtSifre";
            this.txtSifre.PasswordChar = '*';
            this.txtSifre.PrefixSuffixText = null;
            this.txtSifre.ReadOnly = false;
            this.txtSifre.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtSifre.SelectedText = "";
            this.txtSifre.SelectionLength = 0;
            this.txtSifre.SelectionStart = 0;
            this.txtSifre.ShortcutsEnabled = true;
            this.txtSifre.Size = new System.Drawing.Size(228, 48);
            this.txtSifre.TabIndex = 10;
            this.txtSifre.TabStop = false;
            this.txtSifre.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtSifre.TrailingIcon = null;
            this.txtSifre.UseSystemPasswordChar = false;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxLogo.Image")));
            this.pictureBoxLogo.Location = new System.Drawing.Point(158, 67);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(470, 171);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLogo.TabIndex = 11;
            this.pictureBoxLogo.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtSifre);
            this.Controls.Add(this.txtKullaniciAdi);
            this.Controls.Add(this.btnGiris);
            this.Controls.Add(this.pictureBoxLogo);
            this.Name = "Form1";
            this.Text = "CyberLog Vault - Kullanıcı Girişi";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MaterialSkin.Controls.MaterialButton btnGiris;
        private MaterialSkin.Controls.MaterialTextBox txtKullaniciAdi;
        private MaterialSkin.Controls.MaterialTextBox2 txtSifre;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
    }
}