using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using MaterialSkin;
using MaterialSkin.Controls;

namespace CyberLog_Vault
{
    public partial class frmMain : MaterialForm
    {
        // Global değişkenlerimiz
        private string aktifKullanici;
        private int aktifKullaniciID;
        private string aktifYetki;

        private CancellationTokenSource iptalSinyali;
        private CancellationTokenSource webIptalSinyali;

        public frmMain(string kullaniciAdi, string yetki, int kullaniciID)
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;

            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Purple800, Primary.Purple900, Primary.Purple500, Accent.Cyan200, TextShade.WHITE
            );

            this.aktifKullanici = kullaniciAdi;
            this.aktifKullaniciID = kullaniciID;
            this.aktifYetki = yetki;

            this.InitializeControlsAnchor();

            // Tablo Makyajlarını Çalıştır
            if (this.Controls.Find("dgvLoglar", true).FirstOrDefault() != null)
                TabloMakyajla((DataGridView)this.Controls.Find("dgvLoglar", true).First());

            if (this.Controls.Find("dgvKullanicilar", true).FirstOrDefault() != null)
                TabloMakyajla((DataGridView)this.Controls.Find("dgvKullanicilar", true).First());

            this.LoglariGetir();

            this.Text = "CyberLog Vault - Merkezi Kontrol Paneli";

            if (this.Controls.Find("lblKullaniciBilgi", true).FirstOrDefault() != null)
                this.Controls.Find("lblKullaniciBilgi", true).First().Text = $"👤 Kullanıcı: {kullaniciAdi}  |  Yetki: [{yetki}]";

            // =======================================================
            // GÜVENLİK DUVARI: ADMİN DEĞİLSE YÖNETİM BUTONUNU SİL!
            // =======================================================
            string yetkiKontrol = this.aktifYetki.ToLower().Trim();
            if (yetkiKontrol != "admin" && yetkiKontrol != "sistem yoneticisi")
            {
                var btn = this.Controls.Find("btnKullaniciYonetimiMat", true).FirstOrDefault();
                if (btn != null) btn.Parent.Controls.Remove(btn);
            }

            sayfaHosgeldin.BringToFront();
        }

        // =======================================================
        // TABLO MAKYAJI (AKILLI MOTOR - İKİ TABLOYU DA BOZMADAN BOYAR)
        // =======================================================
        private void TabloMakyajla(DataGridView dgv)
        {
            if (dgv == null) return;
            dgv.BackgroundColor = Color.FromArgb(45, 45, 48);
            dgv.BorderStyle = BorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.GridColor = Color.FromArgb(142, 36, 170);
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(74, 20, 140);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(50, 50, 50);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(24, 255, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.RowTemplate.Height = 35;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60);

            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Satırı komple seçmek için eklendi!
        }

        private void InitializeControlsAnchor()
        {
            if (this.Controls.Find("lblKullaniciBilgi", true).FirstOrDefault() != null) this.Controls.Find("lblKullaniciBilgi", true).First().Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            if (this.Controls.Find("btnAnaSayfaMat", true).FirstOrDefault() != null) this.Controls.Find("btnAnaSayfaMat", true).First().Anchor = AnchorStyles.Top | AnchorStyles.Left;
            if (this.Controls.Find("btnAgTaramasiMat", true).FirstOrDefault() != null) this.Controls.Find("btnAgTaramasiMat", true).First().Anchor = AnchorStyles.Top | AnchorStyles.Left;
            if (this.Controls.Find("btnLoglarMat", true).FirstOrDefault() != null) this.Controls.Find("btnLoglarMat", true).First().Anchor = AnchorStyles.Top | AnchorStyles.Left;
            if (this.Controls.Find("btnPortTaramaMat", true).FirstOrDefault() != null) this.Controls.Find("btnPortTaramaMat", true).First().Anchor = AnchorStyles.Top | AnchorStyles.Left;
            if (this.Controls.Find("btnWebIstihbaratMat", true).FirstOrDefault() != null) this.Controls.Find("btnWebIstihbaratMat", true).First().Anchor = AnchorStyles.Top | AnchorStyles.Left;
            if (this.Controls.Find("btnKullaniciYonetimiMat", true).FirstOrDefault() != null) this.Controls.Find("btnKullaniciYonetimiMat", true).First().Anchor = AnchorStyles.Top | AnchorStyles.Left;
            if (this.Controls.Find("btnCikisYapMtl", true).FirstOrDefault() != null) this.Controls.Find("btnCikisYapMtl", true).First().Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Çıkış butonu alta yapışsın

            sayfaHosgeldin.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            sayfaAgTarama.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            sayfaPortTarama.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            sayfaLoglar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            sayfaWebIstihbarat.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            if (this.Controls.Find("sayfaKullaniciYonetimi", true).FirstOrDefault() != null) this.Controls.Find("sayfaKullaniciYonetimi", true).First().Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            if (this.Controls.Find("dgvLoglar", true).FirstOrDefault() != null) this.Controls.Find("dgvLoglar", true).First().Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            if (this.Controls.Find("dgvKullanicilar", true).FirstOrDefault() != null) this.Controls.Find("dgvKullanicilar", true).First().Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        // TASARIMCIYI SUSTURAN BOŞ HAYALET KODLAR
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click_1(object sender, EventArgs e) { }
        private void dgvLoglar_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void btnAdminAvcisi_Click(object sender, EventArgs e) { }
        private void btnKimlikIffsasi_Click(object sender, EventArgs e) { }
        private void btnWebDurdur_Click(object sender, EventArgs e) { }
        private void btnPortTara_Click(object sender, EventArgs e) { }
        private void btnHizliTara_Click_1(object sender, EventArgs e) { }
        private void btnDurdur_Click_1(object sender, EventArgs e) { }
        private void lstWebSonuclar_SelectedIndexChanged(object sender, MaterialListBoxItem selectedItem) { }
        private void btnTara_Click(object sender, EventArgs e) { }
        private void cmbYeniYetki_SelectedIndexChanged(object sender, EventArgs e) { }

        // ========================================================================
        // SQL LOGLAMA MERKEZİ (KİŞİYE ÖZEL)
        // ========================================================================
        private void LogKaydet(string hedefIp, string taramaTipi, string sonuc)
        {
            string baglantiAdresi = @"Data Source=DESKTOP-0AGN3H4;Initial Catalog=NetSecDB;Integrated Security=True";
            using (SqlConnection baglanti = new SqlConnection(baglantiAdresi))
            {
                try
                {
                    baglanti.Open();
                    using (SqlCommand komut = new SqlCommand("INSERT INTO TaramaLoglari (HedefIP, TaramaTipi, Sonuc, TarayanKullaniciID) VALUES (@hedef, @tip, @sonuc, @id)", baglanti))
                    {
                        komut.Parameters.AddWithValue("@hedef", hedefIp); komut.Parameters.AddWithValue("@tip", taramaTipi);
                        komut.Parameters.AddWithValue("@sonuc", sonuc); komut.Parameters.AddWithValue("@id", aktifKullaniciID);
                        komut.ExecuteNonQuery();
                    }
                }
                catch { }
            }
        }

        private void LoglariGetir()
        {
            using (SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-0AGN3H4;Initial Catalog=NetSecDB;Integrated Security=True"))
            {
                try
                {
                    baglanti.Open(); string sorgu = "";
                    string yetkiKontrol = aktifYetki.ToLower().Trim();

                    if (yetkiKontrol == "admin" || yetkiKontrol == "sistem yoneticisi")
                        sorgu = @"SELECT L.LogID AS [Log No], L.HedefIP AS [Taranan Hedef], L.TaramaTipi AS [İşlem], L.Sonuc AS [Durum], L.TaramaZamani AS [Tarih], K.KullaniciAdi AS [Tarayan Kişi] FROM TaramaLoglari L INNER JOIN Kullanicilar K ON L.TarayanKullaniciID = K.KullaniciID ORDER BY L.TaramaZamani DESC";
                    else
                        sorgu = @"SELECT L.LogID AS [Log No], L.HedefIP AS [Taranan Hedef], L.TaramaTipi AS [İşlem], L.Sonuc AS [Durum], L.TaramaZamani AS [Tarih], K.KullaniciAdi AS [Tarayan Kişi] FROM TaramaLoglari L INNER JOIN Kullanicilar K ON L.TarayanKullaniciID = K.KullaniciID WHERE L.TarayanKullaniciID = @id ORDER BY L.TaramaZamani DESC";

                    using (SqlCommand komut = new SqlCommand(sorgu, baglanti))
                    {
                        if (yetkiKontrol != "admin" && yetkiKontrol != "sistem yoneticisi") komut.Parameters.AddWithValue("@id", aktifKullaniciID);
                        using (SqlDataAdapter okuyucu = new SqlDataAdapter(komut))
                        {
                            DataTable sanalTablo = new DataTable(); okuyucu.Fill(sanalTablo);
                            if (this.Controls.Find("dgvLoglar", true).FirstOrDefault() != null) ((DataGridView)this.Controls.Find("dgvLoglar", true).First()).DataSource = sanalTablo;
                        }
                    }
                }
                catch { }
            }
        }

        // ========================================================================
        // ARAÇ: KULLANICI YÖNETİMİ (GÖSTER, EKLE, SİL)
        // ========================================================================
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }

        // TABLOYA KULLANICILARI ÇEKME
        private void KullanicilariGetir()
        {
            using (SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-0AGN3H4;Initial Catalog=NetSecDB;Integrated Security=True"))
            {
                try
                {
                    baglanti.Open();
                    string sorgu = "SELECT KullaniciID AS [ID], KullaniciAdi AS [Kullanıcı Adı], YetkiRolu AS [Yetki/Rol] FROM Kullanicilar";
                    using (SqlDataAdapter okuyucu = new SqlDataAdapter(sorgu, baglanti))
                    {
                        DataTable sanalTablo = new DataTable(); okuyucu.Fill(sanalTablo);
                        if (this.Controls.Find("dgvKullanicilar", true).FirstOrDefault() != null) ((DataGridView)this.Controls.Find("dgvKullanicilar", true).First()).DataSource = sanalTablo;
                    }
                }
                catch { }
            }
        }

        // KULLANICI EKLE
        private void btnKullaniciKaydetMtl_Click(object sender, EventArgs e)
        {
            var txtAd = (MaterialTextBox)this.Controls.Find("txtYeniKullaniciAdi", true).FirstOrDefault();
            var txtSifre = (MaterialTextBox)this.Controls.Find("txtYeniSifre", true).FirstOrDefault();
            var cmbYetki = (MaterialComboBox)this.Controls.Find("cmbYeniYetki", true).FirstOrDefault();

            if (txtAd == null || string.IsNullOrWhiteSpace(txtAd.Text) || string.IsNullOrWhiteSpace(txtSifre.Text) || cmbYetki == null || cmbYetki.SelectedIndex == -1)
            { MessageBox.Show("Lütfen tüm alanları doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string hashliSifre = ComputeSha256Hash(txtSifre.Text);
            using (SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-0AGN3H4;Initial Catalog=NetSecDB;Integrated Security=True"))
            {
                try
                {
                    baglanti.Open();
                    using (SqlCommand kontrol = new SqlCommand("SELECT COUNT(*) FROM Kullanicilar WHERE KullaniciAdi = @kad", baglanti))
                    {
                        kontrol.Parameters.AddWithValue("@kad", txtAd.Text);
                        if ((int)kontrol.ExecuteScalar() > 0) { MessageBox.Show("Bu kullanıcı adı alınmış!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                    }

                    using (SqlCommand komut = new SqlCommand("INSERT INTO Kullanicilar (KullaniciAdi, SifreHash, YetkiRolu) VALUES (@kad, @sifre, @yetki)", baglanti))
                    {
                        komut.Parameters.AddWithValue("@kad", txtAd.Text); komut.Parameters.AddWithValue("@sifre", hashliSifre); komut.Parameters.AddWithValue("@yetki", cmbYetki.Text);
                        komut.ExecuteNonQuery();
                        MessageBox.Show($"[{txtAd.Text}] eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        txtAd.Clear(); txtSifre.Clear(); cmbYetki.SelectedIndex = -1;
                        KullanicilariGetir(); // Eklendikten sonra tabloyu güncelle!
                    }
                }
                catch (Exception hata) { MessageBox.Show("Veritabanı hatası: " + hata.Message, "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        // KULLANICI SİL (YENİ EKLENDİ)
        private void btnKullaniciSilMtl_Click(object sender, EventArgs e)
        {
            var dgv = (DataGridView)this.Controls.Find("dgvKullanicilar", true).FirstOrDefault();
            if (dgv == null || dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz kullanıcıyı tablodan seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int secilenID = Convert.ToInt32(dgv.SelectedRows[0].Cells["ID"].Value);
            string secilenKullanici = dgv.SelectedRows[0].Cells["Kullanıcı Adı"].Value.ToString();

            // GÜVENLİK KURALI: KENDİ KENDİNİ SİLEMEZSİN!
            if (secilenID == aktifKullaniciID)
            {
                MessageBox.Show("Kendi hesabınızı silemezsiniz! Başka bir yönetici hesabından işlem yapın.", "Yetki Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            DialogResult onay = MessageBox.Show($"[{secilenKullanici}] kullanıcısını kalıcı olarak silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (onay == DialogResult.Yes)
            {
                using (SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-0AGN3H4;Initial Catalog=NetSecDB;Integrated Security=True"))
                {
                    try
                    {
                        baglanti.Open();
                        // Kullanıcıya ait eski logları temizlemezsek SQL hata verebilir (Foreign Key constraint)
                        using (SqlCommand logSil = new SqlCommand("DELETE FROM TaramaLoglari WHERE TarayanKullaniciID = @id", baglanti))
                        {
                            logSil.Parameters.AddWithValue("@id", secilenID);
                            logSil.ExecuteNonQuery();
                        }

                        // Kullanıcıyı sil
                        using (SqlCommand komut = new SqlCommand("DELETE FROM Kullanicilar WHERE KullaniciID = @id", baglanti))
                        {
                            komut.Parameters.AddWithValue("@id", secilenID);
                            komut.ExecuteNonQuery();
                            MessageBox.Show("Kullanıcı ve ona ait tüm tarama geçmişi silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            KullanicilariGetir(); // Tabloyu yenile
                        }
                    }
                    catch (Exception hata) { MessageBox.Show("Hata: " + hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        // ÇIKIŞ YAP METODU (YENİ EKLENDİ)
        private void btnCikisYapMtl_Click(object sender, EventArgs e)
        {
            DialogResult onay = MessageBox.Show("Oturumu kapatmak istediğinize emin misiniz?", "Çıkış Yap", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (onay == DialogResult.Yes)
            {
                // Programı sıfırdan ve tertemiz yeniden başlatır!
                Application.Restart();
            }
        }


        // ========================================================================
        // ARAÇ 1: PING (AĞ TARAMASI)
        // ========================================================================
        private void btnTaraMtl_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHedefIP.Text)) { MessageBox.Show("IP girin!"); return; }
            try
            {
                PingReply cevap = new Ping().Send(txtHedefIP.Text, 1500);
                if (cevap != null && cevap.Status == IPStatus.Success) { LogKaydet(txtHedefIP.Text, "Ping Taraması", $"Başarılı ({cevap.RoundtripTime} ms)"); MessageBox.Show($"✅ BAŞARILI\nSüre: {cevap.RoundtripTime} ms", "Sistem Raporu"); }
                else { LogKaydet(txtHedefIP.Text, "Ping Taraması", "Başarısız"); MessageBox.Show("❌ Ulaşılamadı."); }
            }
            catch { MessageBox.Show("Geçersiz IP adresi!"); }
        }

        private void MtlListBoxEkle(MaterialListBox listBox, string yazi)
        {
            listBox.Items.Add(new MaterialListBoxItem(yazi));
            listBox.SelectedIndex = listBox.Items.Count - 1;
        }

        // ========================================================================
        // ARAÇ 2: PORT TARAYICI 
        // ========================================================================
        private async Task PortTaramaMotoru(string hedef, int[] taranacakPortlar, string taramaTipi)
        {
            lstPortSonuclar.Items.Clear();
            MtlListBoxEkle(lstPortSonuclar, $"[*] {hedef} için {taramaTipi} başlatıldı...");
            MtlListBoxEkle(lstPortSonuclar, "--------------------------------------------------");

            if (this.Controls.Find("btnPortTaraMtl", true).FirstOrDefault() != null) this.Controls.Find("btnPortTaraMtl", true).First().Enabled = false;
            if (this.Controls.Find("btnHizliTaraMtl", true).FirstOrDefault() != null) this.Controls.Find("btnHizliTaraMtl", true).First().Enabled = false;
            if (this.Controls.Find("btnDurdurMtl", true).FirstOrDefault() != null) this.Controls.Find("btnDurdurMtl", true).First().Enabled = true;

            int acikPortSayisi = 0; iptalSinyali = new CancellationTokenSource();

            Dictionary<int, string> servisler = new Dictionary<int, string> { { 21, "FTP" }, { 22, "SSH" }, { 80, "HTTP" }, { 443, "HTTPS" }, { 1433, "MSSQL" }, { 3389, "RDP" } };

            foreach (int port in taranacakPortlar)
            {
                if (iptalSinyali.Token.IsCancellationRequested) { MtlListBoxEkle(lstPortSonuclar, "[!] İPTAL EDİLDİ!"); break; }
                try
                {
                    using (TcpClient istemci = new TcpClient())
                    {
                        var baglantiGorevi = istemci.ConnectAsync(hedef, port);
                        if (await Task.WhenAny(baglantiGorevi, Task.Delay(100)) == baglantiGorevi && istemci.Connected)
                        {
                            string sAd = servisler.ContainsKey(port) ? servisler[port] : "Bilinmeyen";
                            MtlListBoxEkle(lstPortSonuclar, $"[+] PORT {port} AÇIK! --> Servis: {sAd}");
                            acikPortSayisi++;
                        }
                    }
                }
                catch { }
            }
            MtlListBoxEkle(lstPortSonuclar, $"[*] Tarama Bitti. {acikPortSayisi} açık port bulundu.");

            if (this.Controls.Find("btnPortTaraMtl", true).FirstOrDefault() != null) this.Controls.Find("btnPortTaraMtl", true).First().Enabled = true;
            if (this.Controls.Find("btnHizliTaraMtl", true).FirstOrDefault() != null) this.Controls.Find("btnHizliTaraMtl", true).First().Enabled = true;
            if (this.Controls.Find("btnDurdurMtl", true).FirstOrDefault() != null) this.Controls.Find("btnDurdurMtl", true).First().Enabled = false;

            LogKaydet(hedef, taramaTipi, $"{acikPortSayisi} Açık Port");
        }

        private async void btnPortTaraMtl_Click(object sender, EventArgs e) { if (string.IsNullOrWhiteSpace(txtPortHedef.Text)) return; await PortTaramaMotoru(txtPortHedef.Text, Enumerable.Range(1, 1000).ToArray(), "Derin Tarama"); }
        private async void btnHizliTaraMtl_Click(object sender, EventArgs e) { if (string.IsNullOrWhiteSpace(txtPortHedef.Text)) return; await PortTaramaMotoru(txtPortHedef.Text, new int[] { 21, 22, 23, 25, 53, 80, 110, 135, 139, 443, 445, 1433, 3306, 3389 }, "Hızlı Tarama"); }
        private void btnDurdurMtl_Click(object sender, EventArgs e) { iptalSinyali?.Cancel(); }

        // ========================================================================
        // ARAÇ 3: WEB İSTİHBARAT & WAF
        // ========================================================================
        private async void btnKimlikIffsasiMtl_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtWebHedef.Text)) return;
            string hedef = txtWebHedef.Text.StartsWith("http") ? txtWebHedef.Text : "http://" + txtWebHedef.Text;
            lstWebSonuclar.Items.Clear();
            MtlListBoxEkle(lstWebSonuclar, $"[*] {hedef} WAF Tespiti...");

            try
            {
                using (HttpClient istemci = new HttpClient())
                {
                    istemci.DefaultRequestHeaders.Add("User-Agent", "CyberLog_Recon");
                    HttpResponseMessage cevap = await istemci.GetAsync(hedef, HttpCompletionOption.ResponseHeadersRead);
                    MtlListBoxEkle(lstWebSonuclar, "[+] HEDEF YANIT VERDİ!");

                    bool wafBulundu = false; string basliklar = cevap.Headers.ToString().ToLower();
                    Dictionary<string, string> waflar = new Dictionary<string, string> { { "cloudflare", "Cloudflare" }, { "sucuri", "Sucuri" }, { "incapsula", "Imperva" }, { "gws", "Google" } };

                    foreach (var imza in waflar) { if (basliklar.Contains(imza.Key)) { MtlListBoxEkle(lstWebSonuclar, $"[!!!] WAF TESPİT EDİLDİ: {imza.Value}"); wafBulundu = true; break; } }
                    if (!wafBulundu) MtlListBoxEkle(lstWebSonuclar, "[+] WAF Yok.");
                    LogKaydet(txtWebHedef.Text, "WAF Analizi", wafBulundu ? "WAF Tespit Edildi" : "WAF Yok");
                }
            }
            catch (Exception hata) { MtlListBoxEkle(lstWebSonuclar, "Hata: " + hata.Message); }
        }

        private async void materialButton1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtWebHedef.Text)) return;
            string hedef = txtWebHedef.Text.Replace("http://", "").Replace("https://", "").TrimEnd('/');
            string[] wordlist = { "admin", "login", "cpanel", "test", "wp-admin" };
            lstWebSonuclar.Items.Clear(); MtlListBoxEkle(lstWebSonuclar, $"[*] {hedef} Kapsamlı Av Başladı...");

            btnKimlikIffsasiMtl.Enabled = false;
            if (this.Controls.Find("materialButton1", true).FirstOrDefault() != null) this.Controls.Find("materialButton1", true).First().Enabled = false;
            btnWebDurdurMtl.Enabled = true;

            int bulunanSayisi = 0; webIptalSinyali = new CancellationTokenSource();

            using (HttpClient istemci = new HttpClient())
            {
                istemci.Timeout = TimeSpan.FromSeconds(2);
                foreach (string kelime in wordlist)
                {
                    if (webIptalSinyali.Token.IsCancellationRequested) { MtlListBoxEkle(lstWebSonuclar, "[!] İPTAL EDİLDİ!"); break; }
                    string[] urller = { $"http://{kelime}.{hedef}", $"http://{hedef}/{kelime}" };
                    foreach (string url in urller)
                    {
                        try
                        {
                            HttpResponseMessage cvp = await istemci.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                            if (cvp.IsSuccessStatusCode || cvp.StatusCode == System.Net.HttpStatusCode.Forbidden)
                            { MtlListBoxEkle(lstWebSonuclar, $"[+++] BİNGO: {url}"); bulunanSayisi++; }
                        }
                        catch { }
                    }
                }
            }
            MtlListBoxEkle(lstWebSonuclar, $"[*] Bitti. {bulunanSayisi} hedef bulundu.");
            btnKimlikIffsasiMtl.Enabled = true;
            if (this.Controls.Find("materialButton1", true).FirstOrDefault() != null) this.Controls.Find("materialButton1", true).First().Enabled = true;
            btnWebDurdurMtl.Enabled = false;

            LogKaydet(hedef, "Panel Avcısı", $"{bulunanSayisi} Hedef");
        }

        private void btnWebDurdurMtl_Click(object sender, EventArgs e) { webIptalSinyali?.Cancel(); }

        // ========================================================================
        // SOL MENÜ MATERIAL BUTONLARIN SAYFA GEÇİŞ KODLARI
        // ========================================================================
        private void materialButton1_Click(object sender, EventArgs e) { sayfaHosgeldin.BringToFront(); }
        private void btnAgTaramasiMat_Click(object sender, EventArgs e) { sayfaAgTarama.BringToFront(); }
        private void btnPortTaramaMat_Click(object sender, EventArgs e) { sayfaPortTarama.BringToFront(); }
        private void btnLoglarMat_Click(object sender, EventArgs e) { sayfaLoglar.BringToFront(); LoglariGetir(); }
        private void btnWebIstihbaratMat_Click(object sender, EventArgs e) { sayfaWebIstihbarat.BringToFront(); }

        private void btnKullaniciYonetimiMat_Click(object sender, EventArgs e)
        {
            if (this.Controls.Find("sayfaKullaniciYonetimi", true).FirstOrDefault() != null)
            {
                this.Controls.Find("sayfaKullaniciYonetimi", true).First().BringToFront();
                KullanicilariGetir(); // Sayfayı açınca tabloyu doldur!
            }
        }
    }
}