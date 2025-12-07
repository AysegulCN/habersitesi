# YILDIZ HABER PORTALI (ASP.NET CORE MVC PROJESİ)

Bu proje, İnternet Programcılığı dersi Ara Sınav teslimi için hazırlanmış, temel içerik yönetim sistemine (CMS) sahip bir Yönetici Paneli uygulamasıdır. 

## 🚀 TEKNOLOJİLER
- **Framework:** ASP.NET Core 7/8 (MVC)
- **Veritabanı:** SQL Server (Entity Framework Core ile)
- **Tasarım:** SB Admin 2 (Bootstrap)
- **Editor:** TinyMCE (Zengin Metin Editörü)
- **Diğer:** jQuery, AJAX (Asenkron İşlemler)

## ✅ PROJE ÖZELLİKLERİ (Ara Sınav Kapsamı)
Proje, aşağıdaki kritik maddeleri içermektedir:

1.  **CRUD İşlemleri:** Haberler ve Kategoriler için Oluşturma, Okuma, Güncelleme, Silme (C-R-U-D) işlemleri tamdır.
2.  **AJAX Kullanımı:** Haber ve Kategori listelerinde **Geri Alma (Undo)** özellikli AJAX silme metodu kullanılmıştır.
3.  **Dinamik Menü:** Kategoriler veritabanından çekilerek menüye otomatik olarak yüklenmektedir.
4.  **Mimarî:** Yönetici Yüzü (`AdminController`) ve Genel Yüz (`HomeController`) ayrımı yapılmıştır.

## ⚙️ KURULUM VE ÇALIŞTIRMA

1.  Projeyi yerel diskinize klonlayın.
2.  Visual Studio'da projeyi açın.
3.  **Veritabanı Bağlantısı:** `appsettings.json` dosyasındaki ConnectionString'i kendi SQL Server ayarlarınıza göre güncelleyin.
4.  **Migration:** Package Manager Console'da `Update-Database` komutunu çalıştırarak veritabanı şemasını oluşturun.
5.  Uygulamayı çalıştırın (F5 veya Ctrl+F5).
6.  Yönetici paneli giriş adresi: `localhost:7231/Admin/Index`
