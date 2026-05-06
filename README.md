# 🛡️ CyberLog Vault

**CyberLog Vault**, siber güvenlik operasyonlarını, ağ taramalarını ve web istihbarat süreçlerini tek bir merkezden yönetmek ve kayıt altına almak için geliştirilmiş bir **C# Windows Forms** projesidir. Sistem, arka planda **Microsoft SQL Server** kullanarak tüm işlemleri güvenli bir şekilde (loglayarak) veritabanında tutar.

Görsel arayüz olarak modern ve karanlık bir siberpunk teması sunan **MaterialSkin** kütüphanesi kullanılmıştır.

<p align="center">
  <img src="https://github.com/user-attachments/assets/559a2ab6-70db-4d69-bf3e-a974bc8f104b" width="800">
</p>
<p align="center">
  <img src="https://github.com/user-attachments/assets/310a5818-2dc7-4f57-aaaf-4d15a5e88077" width="800">
</p>
  <img src="https://github.com/user-attachments/assets/966c196a-2e48-4e82-8b96-47038bdb5eb2" width="800">
</p>
## 🚀 Temel Özellikler

Sistem içerisinde hayali veya çalışmayan hiçbir fonksiyon yoktur. Aşağıdaki tüm araçlar aktif olarak çalışmaktadır:

### 1. Kriptografik Kimlik Doğrulama & RBAC
* **SHA-256 Şifreleme:** Kullanıcı şifreleri veritabanında düz metin (plain-text) olarak tutulmaz, SHA-256 algoritmasıyla şifrelenir.
* **Rol Bazlı Erişim (RBAC):** `Admin` ve `User` yetkilendirmesi mevcuttur. Adminler tüm sistem loglarını görebilir ve yeni kullanıcı ekleyip silebilirken; normal kullanıcılar sadece kendi yaptıkları tarama loglarını görebilir.

### 2. Ağ ve Port Analiz Araçları
* **Ping (Ağ Taraması):** Hedef IP adresine paket göndererek erişilebilirliği ve gecikme süresini (ms) ölçer.
* **Asenkron Port Tarayıcı:** Hedef sistemdeki açık portları tespit eder. Hızlı Tarama (Kritik portlar) ve Derin Tarama (1-1000 arası) olmak üzere iki farklı mod sunar.

### 3. Web İstihbarat (OSINT) & WAF Tespiti
* **WAF Dedektörü:** Hedef web sitesinin HTTP başlıklarını (Headers) analiz ederek arkasında Cloudflare, Sucuri, Imperva gibi bir Web Uygulama Güvenlik Duvarı (WAF) olup olmadığını tespit eder.
* **Admin Panel Avcısı:** Belirlenen hedef üzerinde yaygın olarak kullanılan kritik dizinleri (örn: `/admin`, `/login`, `cpanel`) asenkron HTTP istekleriyle tarayarak açıkta olan panelleri ifşa eder.

### 4. Güvenli Veri Yönetimi (CRUD)
* **SQL Injection Koruması:** Veritabanına yapılan tüm kayıt (INSERT) ve sorgu (SELECT) işlemlerinde `Parameterized Queries` kullanılarak sızıntıların önüne geçilmiştir.
* **İlişkisel Loglama:** Yapılan her tarama, işlemi yapan kişinin ID'si ile (Foreign Key) veritabanına mühürlenir. 
* **Kullanıcı Yönetimi:** Adminler tarafından sistemden bir kullanıcı silindiğinde, veritabanı bütünlüğünü korumak için (Cascade Delete mantığıyla) önce o kullanıcının logları, sonra kendisi silinir.

---

## 🛠️ Kullanılan Teknolojiler
* **Programlama Dili:** C# (.NET Framework)
* **Veritabanı:** Microsoft SQL Server (ADO.NET)
* **Arayüz (UI):** MaterialSkin (Dark Theme)
* **Ağ & Web Sınıfları:** `System.Net.NetworkInformation`, `System.Net.Sockets.TcpClient`, `System.Net.Http.HttpClient`

---

## ⚙️ Veritabanı Kurulumu

Projeyi kendi bilgisayarınızda çalıştırmak için SQL Server üzerinde aşağıdaki tabloları oluşturmanız gerekmektedir:
(Bağlantı dizesini (Connection String) frmMain.cs ve Form1.cs içerisinden kendi SQL Server adınıza göre güncellemeyi unutmayın).

```sql
-- 1. Kullanıcılar Tablosu
CREATE TABLE Kullanicilar (
    KullaniciID INT PRIMARY KEY IDENTITY(1,1),
    KullaniciAdi NVARCHAR(50) NOT NULL UNIQUE,
    SifreHash NVARCHAR(MAX) NOT NULL,
    YetkiRolu NVARCHAR(50) NOT NULL
);

-- 2. Tarama Logları Tablosu
CREATE TABLE TaramaLoglari (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    HedefIP NVARCHAR(100),
    TaramaTipi NVARCHAR(50),
    Sonuc NVARCHAR(MAX),
    TaramaZamani DATETIME DEFAULT GETDATE(),
    TarayanKullaniciID INT FOREIGN KEY REFERENCES Kullanicilar(KullaniciID)
);
