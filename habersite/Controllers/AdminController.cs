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

        // --- TEMEL YÖNETİM SAYFALARI ---

        public IActionResult Index()
        {
            ViewData["Title"] = "Yönetim Paneli Ana Sayfa";
            return View();
        }

        // --- HABER LİSTESİ ---
        public IActionResult NewsList()
        {
            try
            {
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
                if (file != null && file.Length > 0)
                {
                    news.ImageId = file.FileName;
                    news.ImageUrl = $"/images/{file.FileName}";
                }

                news.CreatedDate = DateTime.Now;
                _context.News.Add(news);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Haber başarıyla eklendi ve yayınlandı!";
                return RedirectToAction("NewsList");
            }

            LoadCategoriesForViewBag();
            ViewData["Title"] = "Yeni Haber Ekle";
            return View(news);
        }

        // --- HABER DÜZENLEME (GET) ---
        public IActionResult EditNews(int? id)
        {
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
            ViewData["Title"] = "Kategori Yönetimi";
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        // --- KATEGORİ EKLEME (GET) ---
        public IActionResult CreateCategory()
        {
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
                _context.Categories.Update(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Kategori başarıyla güncellendi!";
                return RedirectToAction("CategoryList");
            }
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

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategoryConfirmed(int id)
        {
            var categoryFromDb = _context.Categories.Find(id);
            if (categoryFromDb == null)
            {
                TempData["Error"] = "Silinecek kategori bulunamadı.";
                return RedirectToAction("CategoryList");
            }

            _context.Categories.Remove(categoryFromDb);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Kategori başarıyla silindi!";
            return RedirectToAction("CategoryList");
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


        public IActionResult Ekonomi() { ViewData["Title"] = "Ekonomi Haberleri"; return View(); }
        public IActionResult Dunya() { ViewData["Title"] = "Dünya Haberleri"; return View(); }
        public IActionResult Spor() { ViewData["Title"] = "Spor Haberleri"; return View(); }
        public IActionResult Kadin() { ViewData["Title"] = "Kadın Haberleri"; return View(); }
        public IActionResult Teknoloji() { ViewData["Title"] = "Teknoloji Haberleri"; return View(); }


        public IActionResult Login() { ViewData["Title"] = "Giriş Yap"; return View("~/Views/Account/Login.cshtml"); }
        public IActionResult Register() { ViewData["Title"] = "Kayıt Ol"; return View("~/Views/Account/Register.cshtml"); }
        public IActionResult ForgotPassword() { ViewData["Title"] = "Şifremi Unuttum"; return View("~/Views/Account/ForgotPassword.cshtml"); }
        public IActionResult Page404() { ViewData["Title"] = "Sayfa Bulunamadı"; return View("~/Views/Account/Page404.cshtml"); }

    }
}