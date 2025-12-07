using Microsoft.AspNetCore.Mvc;
using habersite.Models;
using Microsoft.EntityFrameworkCore; // DbContext'in tanınması için kritik!
using System.Linq; // LINQ ve Query işlemleri için
using Microsoft.AspNetCore.Http; // IFormFile için
using System; // DateTime için

namespace habersite.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- YARDIMCI METOT ---
        private void LoadCategoriesForViewBag()
        {
            try
            {
                var categoriesList = _context.Categories
                    .Where(c => c.IsActive)
                    .ToList();

                ViewBag.Categories = categoriesList ?? new List<Category>();
            }
            catch (Exception ex)
            {
                ViewBag.Categories = new List<Category>();
                Console.WriteLine($"Kategori getirme hatası: {ex.Message}");
            }
        }


        // --- YARDIMCI METOT: Başlıktan URL Dostu Slug Oluşturma ---
        public static string CreateSlug(string title)
        {
            string slug = title.ToLower();
            slug = slug.Replace("ş", "s").Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ö", "o").Replace("ç", "c");

            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", ""); 
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", " ").Trim(); 
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s", "-"); 

            return slug;
        }

        // --- TEMEL YÖNETİM SAYFALARI ---

        public void LoadAllCategoriesForLayout()
        {
            var categoriesList = _context.Categories.ToList();
            ViewData["AllCategories"] = categoriesList;
        }

        public IActionResult Index()
        {
            LoadAllCategoriesForLayout();
            ViewData["Title"] = "Yönetim Paneli Ana Sayfa";
            return View();
        }

        // --- HABER LİSTESİ ---
        public IActionResult NewsList()
        {
            try
            {
                LoadAllCategoriesForLayout();
                var news = _context.News.Include(n => n.Category).ToList();
                return View(news);
            }
            catch
            {
                return View(new List<News>());
            }
        }

        // --- HABER EKLEME (GET) ---
        public IActionResult CreateNews()
        {
            LoadAllCategoriesForLayout();
            LoadCategoriesForViewBag();
            ViewData["Title"] = "Yeni Haber Ekle";
            return View();
        }

        // --- HABER EKLEME (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNews(News news, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // 1. Resim İşlemleri
                if (file != null && file.Length > 0)
                {
                    news.ImageId = file.FileName;
                    news.ImageUrl = $"/images/{file.FileName}";
                }

                // 2. KRİTİK ADIM: Slug oluşturuluyor ve Model'e atanıyor
                news.Slug = CreateSlug(news.Title);

                // 3. Veritabanına Ekleme
                news.CreatedDate = DateTime.Now;
                _context.News.Add(news);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Haber başarıyla eklendi ve yayınlandı!";
                return RedirectToAction("NewsList");
            }

            // Model geçersizse menüyü ve kategorileri tekrar yükle
            LoadAllCategoriesForLayout(); // Layout için dinamik menü yüklensin
            LoadCategoriesForViewBag();
            ViewData["Title"] = "Yeni Haber Ekle";
            return View(news);
        }

        // --- HABER DÜZENLEME (GET) ---
        public IActionResult EditNews(int? id)
        {
            LoadAllCategoriesForLayout();
            if (id == null || id == 0) return NotFound();
            var newsItem = _context.News.Find(id);
            if (newsItem == null) return NotFound();

            LoadCategoriesForViewBag();
            ViewData["Title"] = $"Haber Düzenle: {newsItem.Title}";
            return View(newsItem);
        }

        // --- HABER DÜZENLEME (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditNews(News news, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // NOT: Gerçek projede resim değişimi/yüklemesi burada işlenmelidir.
                _context.News.Update(news);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Haber başarıyla güncellendi!";
                return RedirectToAction("NewsList");
            }

            LoadCategoriesForViewBag();
            ViewData["Title"] = $"Haber Düzenle: {news.Title}";
            return View(news);
        }

        // --- HABER SİLME (POST - AJAX Uyumlu) ---
        [HttpPost]
        // [ValidateAntiForgeryToken] // AJAX ile gönderim yapıyorsanız bu satırı kaldırmanız gerekebilir.
        public IActionResult DeleteNews(int id)
        {
            var newsItem = _context.News.Find(id);

            if (newsItem == null)
            {
                return Json(new { success = false, message = "Silinecek haber bulunamadı." });
            }

            _context.News.Remove(newsItem);
            _context.SaveChanges();

            // AJAX'a başarı mesajı döndür
            return Json(new { success = true, message = "Haber başarıyla silindi." });
        }


        // --- KATEGORİ YÖNETİMİ ---

        public IActionResult CategoryList()
        {
            LoadAllCategoriesForLayout();
            ViewData["Title"] = "Kategori Yönetimi";
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        


        // --- KATEGORİ EKLEME (GET) ---
        public IActionResult CreateCategory()
        {
            LoadAllCategoriesForLayout();
            ViewData["Title"] = "Yeni Kategori Ekle";
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedDate = DateTime.Now;
                _context.Categories.Add(category);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Kategori başarıyla eklendi!";
                return RedirectToAction("CategoryList");
            }

            ViewData["Title"] = "Yeni Kategori Ekle";
            return View(category);
        }


        public IActionResult EditCategory(int? id)
        {
            LoadAllCategoriesForLayout();
            if (id == null || id == 0) return NotFound();
            var categoryFromDb = _context.Categories.Find(id);
            if (categoryFromDb == null) return NotFound();
            ViewData["Title"] = $"Kategori Düzenle: {categoryFromDb.Name}";
            return View(categoryFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("CategoryList");
            }

            LoadAllCategoriesForLayout();
            ViewData["Title"] = $"Kategori Düzenle: {category.Name}";
            return View(category);
        }
        public IActionResult DeleteCategory(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var categoryFromDb = _context.Categories.Find(id);
            if (categoryFromDb == null) return NotFound();
            ViewData["Title"] = $"Kategori Sil: {categoryFromDb.Name}";
            return View(categoryFromDb);
        }
        // --- KATEGORİ SİLME (POST - AJAX Uyumlu) ---
        [HttpPost, ActionName("DeleteCategory")]
        public IActionResult DeleteCategoryConfirmed(int id)
        {
            var categoryFromDb = _context.Categories.Find(id);

            if (categoryFromDb == null)
            {
                // AJAX'a hata mesajı gönder
                return Json(new { success = false, message = "Silinecek kategori bulunamadı." });
            }

            // Haberlerin silinmesi gereken ek mantık buraya gelebilir.
            _context.Categories.Remove(categoryFromDb);
            _context.SaveChanges();

            // Başarılı JSON yanıtı gönder
            return Json(new { success = true, message = "Kategori başarıyla silindi." });
        }


        // --- HABERİ GERİ YÜKLEME (RESTORE) ---
        [HttpPost]
        public IActionResult RestoreNews(int id)
        {
            // 1. Haberi ID ile bul
            var newsItem = _context.News.Find(id);

            if (newsItem == null)
            {
                return Json(new { success = false, message = "Geri yüklenecek haber bulunamadı." });
            }

            // 2. Haber Aktiflik/Silinme Bayrağını geri al (VERİTABANI İŞLEMİ)
            // Eğer modelinizde 'IsActive' alanı varsa:
            newsItem.IsActive = true;

            // 3. Değişiklikleri kaydet
            _context.SaveChanges();

            // 4. AJAX'a başarı mesajı dön
            return Json(new { success = true, message = "Haber başarıyla geri yüklendi." });
        }



        // --- KATEGORİ VE SİSTEM SAYFALARI ---

        public IActionResult Last()
        {
            LoadAllCategoriesForLayout();
            ViewData["Title"] = "Son Dakika Haberleri";

            // Modelde IsBreakingNews özelliği olana kadar, NewsList mantığını kullanıyoruz.
            try
            {
                var news = _context.News.Include(n => n.Category).ToList();
                // İleride: .Where(n => n.IsBreakingNews == true).ToList() eklenecek.
                return View(news);
            }
            catch
            {
                return View(new List<News>());
            }
        } 


        public IActionResult Raporlar()
        {
            LoadAllCategoriesForLayout();
            ViewData["Title"] = "Site İstatistikleri ve Raporlar";
            return View();
        }
       
        [HttpGet]
        public IActionResult AddCategoriesNow()
        {
            // Invalid object name 'Categories' hatası sonrası DB'ye veri eklemek için kullanıldı.
            _context.Database.ExecuteSqlRaw(@"
        IF NOT EXISTS (SELECT * FROM Categories WHERE Name = 'Ekonomi')
        BEGIN
            INSERT INTO Categories (Name, Description, IsActive, CreatedDate) VALUES
            ('Ekonomi', 'Ekonomi haberleri', 1, GETDATE()),
            ('Dünya', 'Dünya haberleri', 1, GETDATE()),
            ('Spor', 'Spor haberleri', 1, GETDATE()),
            ('Kadın', 'Kadın haberleri', 1, GETDATE()),
            ('Teknoloji', 'Teknoloji haberleri', 1, GETDATE());
        END
        ");

            return RedirectToAction("CreateNews");
        }


        public IActionResult Ekonomi() { LoadAllCategoriesForLayout(); ViewData["Title"] = "Ekonomi Haberleri"; return View(); }
        public IActionResult Dunya() { LoadAllCategoriesForLayout(); ViewData["Title"] = "Dünya Haberleri"; return View(); }
        public IActionResult Spor() { LoadAllCategoriesForLayout();  ViewData["Title"] = "Spor Haberleri"; return View(); }
        public IActionResult Kadin() { LoadAllCategoriesForLayout(); ViewData["Title"] = "Kadın Haberleri"; return View(); }
        public IActionResult Teknoloji() { LoadAllCategoriesForLayout(); ViewData["Title"] = "Teknoloji Haberleri"; return View(); }


        public IActionResult Login() { LoadAllCategoriesForLayout(); ViewData["Title"] = "Giriş Yap"; return View("~/Views/Account/Login.cshtml"); }
        public IActionResult Register() { LoadAllCategoriesForLayout(); ViewData["Title"] = "Kayıt Ol"; return View("~/Views/Account/Register.cshtml"); }
        public IActionResult ForgotPassword() { LoadAllCategoriesForLayout(); ViewData["Title"] = "Şifremi Unuttum"; return View("~/Views/Account/ForgotPassword.cshtml"); }
        public IActionResult Page404() { LoadAllCategoriesForLayout(); ViewData["Title"] = "Sayfa Bulunamadı"; return View("~/Views/Account/Page404.cshtml"); }

    }
}