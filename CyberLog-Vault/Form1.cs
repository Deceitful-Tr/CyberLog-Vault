using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Linq;

namespace CyberLog_Vault
{
    // DİKKAT: Sende formun adı Form1 olduğu için burayı Form1 yaptık!
    public partial class Form1 : MaterialForm
    {
        public Form1()
        {
            InitializeComponent();

            // =======================================================
            // MATERİAL SKİN TEMA AYARLARI (SİMSİYAH HACKER TEMASI)
            // =======================================================
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;

            // Yeni Cyberpunk Paleti: Deep Mor, Koyu Mor, Siber Cyan Accent
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Purple800,
                Primary.Purple900,
                Primary.Purple500,
                Accent.Cyan200, // Vurgu rengi matrix mavisi!
                TextShade.WHITE
            );

            // Responsive Merkezleme Metodunu Çağırıyoruz
            this.InitializeControlsAnchor();
        }

        // YENİ METOT: Login ekranındaki öğeleri merkeze çıpalamak
        private void InitializeControlsAnchor()
        {
            // AnchorStyles.None yaparsak, pencere büyüdüğünde öğeler esnemez, tam ortada asılı kalır. (Tam Login ekranı mantığı)

            // LOGO ÇIPASI (YENİ EKLENDİ)
            if (this.Controls.Find("pictureBoxLogo", true).FirstOrDefault() != null)
                this.Controls.Find("pictureBoxLogo", true).First().Anchor = AnchorStyles.None;

            if (this.Controls.Find("txtKullaniciAdi", true).Length > 0)
                this.Controls.Find("txtKullaniciAdi", true).First().Anchor = AnchorStyles.None;

            if (this.Controls.Find("txtSifre", true).Length > 0)
                this.Controls.Find("txtSifre", true).First().Anchor = AnchorStyles.None;

            if (this.Controls.Find("btnGiris", true).Length > 0)
                this.Controls.Find("btnGiris", true).First().Anchor = AnchorStyles.None;
        }

        // Giriş Butonu Tıklanma Olayı
        private void btnGiris_Click(object sender, EventArgs e)
        {
            // 1. Kutular boş bırakılmış mı kontrol et
            if (string.IsNullOrWhiteSpace(txtKullaniciAdi.Text) || string.IsNullOrWhiteSpace(txtSifre.Text))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifrenizi giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Şifreyi Hash'le
            string girilenSifreHash = ComputeSha256Hash(txtSifre.Text);

            // 3. SQL Bağlantısı
            string baglantiAdresi = @"Data Source=DESKTOP-0AGN3H4;Initial Catalog=NetSecDB;Integrated Security=True";

            using (SqlConnection baglanti = new SqlConnection(baglantiAdresi))
            {
                try
                {
                    baglanti.Open();
                    string sorgu = "SELECT KullaniciID, YetkiRolu FROM Kullanicilar WHERE KullaniciAdi = @kAd AND SifreHash = @sifre";

                    using (SqlCommand komut = new SqlCommand(sorgu, baglanti))
                    {
                        komut.Parameters.AddWithValue("@kAd", txtKullaniciAdi.Text);
                        komut.Parameters.AddWithValue("@sifre", girilenSifreHash);

                        using (SqlDataReader okuyucu = komut.ExecuteReader())
                        {
                            if (okuyucu.Read())
                            {
                                string yetki = okuyucu["YetkiRolu"].ToString();
                                int kullaniciID = Convert.ToInt32(okuyucu["KullaniciID"]);

                                MessageBox.Show($"Giriş Başarılı!\nHoş geldin, {txtKullaniciAdi.Text} ({yetki})", "Sistem Onayı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Başarılı girişten sonra ana forma geçiş (İsimler jilet gibi bağlandı)
                                frmMain anaPanel = new frmMain(txtKullaniciAdi.Text, yetki, kullaniciID);
                                anaPanel.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Kullanıcı adı veya şifre hatalı!", "Giriş Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception hata)
                {
                    MessageBox.Show("Veri tabanına bağlanırken bir hata oluştu:\n" + hata.Message, "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --- ŞİFREYİ HASH'LEME FONKSİYONU ---
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}