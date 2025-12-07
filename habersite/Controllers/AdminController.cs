using Microsoft.AspNetCore.Mvc;
using habersite.Models;
using Microsoft.EntityFrameworkCore; // DbContext'in tanınması için kritik!
using System.Linq; // LINQ ve Query işlemleri için

namespace habersite.Controllers
{
    public class AdminController : Controller
    {
        // 1. VERİTABANI BAĞLANTISI (Dependency Injection için gerekli alan)
        private readonly ApplicationDbContext _context;

        // 2. CONSTRUCTOR (Yapıcı Metot) ile Context'i enjekte etme
        // Bu, Context'i tanımanızı sağlayan kritik kısımdır.

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- TEMEL YÖNETİM SAYFALARI ---

        public IActionResult Index()
        {
            ViewData["Title"] = "Yönetim Paneli Ana Sayfa";
            // TODO: Haber istatistikleri buraya çekilecek
            return View();
        }

        public IActionResult NewsList()
        {
            try
            {
                var news = _context.News.Include(n => n.Category).ToList();
                return View(news);
            }
            catch
            {
                // Database yoksa boş liste göster
                return View(new List<News>());
            }
        }

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
            // Resim yükleme ve model geçerliliği kontrolü yapılır.
            if (ModelState.IsValid)
            {
                // Resim Yükleme (Basitleştirilmiş)
                if (file != null && file.Length > 0)
                {
                    // Gerçek projede: Resmi wwwroot'a kaydetme kodu buraya gelir.
                    // Şimdilik sadece resim dosya adını alıp modele kaydedelim
                    news.ImageId = file.FileName;
                    news.ImageUrl = $"/images/{file.FileName}"; // Örnek yol
                }

                // Veritabanına Ekle
                news.CreatedDate = DateTime.Now;
                _context.News.Add(news);

                // Değişiklikleri Kaydet
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Haber başarıyla eklendi ve yayınlandı!";
                return RedirectToAction("NewsList");
            }

            // Model geçerli değilse, kategorileri tekrar yükle ve formu geri gönder
            LoadCategoriesForViewBag();
            ViewData["Title"] = "Yeni Haber Ekle";
            return View(news);
        }






        // --- Yardımcı Metot: Kategorileri Yükleme ---
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


        [HttpPost]
        [ValidateAntiForgeryToken] // Güvenlik için önerilir
        public IActionResult CreateCategory(Category category)
        {
            // 1. Model Geçerliliğini Kontrol Et
            if (ModelState.IsValid)
            {
                // 2. Veritabanına Ekle
                category.CreatedDate = DateTime.Now; // Oluşturma tarihini ayarla
                _context.Categories.Add(category);

                // 3. Değişiklikleri Kaydet
                _context.SaveChanges();

                // 4. Başarılı İşlem Sonrası Yönlendirme
                TempData["SuccessMessage"] = "Kategori başarıyla eklendi!";
                return RedirectToAction("CategoryList"); // Kategoriler listesine yönlendir
            }

            // Model geçerli değilse, formu hatalarla birlikte tekrar göster
            ViewData["Title"] = "Yeni Kategori Ekle";
            return View(category);
        }
        public IActionResult CategoryList()
        {
            ViewData["Title"] = "Kategori Yönetimi";
            var categories = _context.Categories.ToList();
            return View(categories);
        }
        public IActionResult EditCategory(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound(); // ID yoksa 404 döndür
            }

            var categoryFromDb = _context.Categories.Find(id);

            if (categoryFromDb == null)
            {
                return NotFound(); // Kategori veritabanında bulunamazsa 404 döndür
            }

            ViewData["Title"] = $"Kategori Düzenle: {categoryFromDb.Name}";
            return View(categoryFromDb); // EditCategory.cshtml View'ine kategoriyi gönder
        }

        // --- KATEGORİ DÜZENLEME (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category); // Var olan kategoriyi güncelle
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Kategori başarıyla güncellendi!";
                return RedirectToAction("CategoryList");
            }

            ViewData["Title"] = $"Kategori Düzenle: {category.Name}";
            return View(category);
        }

        // --- KATEGORİ SİLME (GET) ---
        public IActionResult DeleteCategory(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var categoryFromDb = _context.Categories.Find(id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            ViewData["Title"] = $"Kategori Sil: {categoryFromDb.Name}";
            return View(categoryFromDb); // DeleteCategory.cshtml View'ine kategoriyi gönder
        }

        // --- KATEGORİ SİLME (POST) ---
        [HttpPost, ActionName("DeleteCategory")] // ActionName, aynı isme sahip iki metodu ayırır
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategoryConfirmed(int id)
        {
            var categoryFromDb = _context.Categories.Find(id);

            if (categoryFromDb == null)
            {
                TempData["Error"] = "Silinecek kategori bulunamadı.";
                return RedirectToAction("CategoryList");
            }

            _context.Categories.Remove(categoryFromDb); // Kategoriyi kaldır
            _context.SaveChanges(); // Değişiklikleri kaydet

            TempData["SuccessMessage"] = "Kategori başarıyla silindi!";
            return RedirectToAction("CategoryList");
        }


        public IActionResult Last()
        {
            ViewData["Title"] = "Son Dakika Haberleri";
            return View();
        }

        // --- RAPORLAR ---
        public IActionResult Raporlar()
        {
            ViewData["Title"] = "Site İstatistikleri ve Raporlar";
            return View();
        }

        // --- KATEGORİ SAYFALARI ---
        public IActionResult Ekonomi()
        {
            ViewData["Title"] = "Ekonomi Haberleri";
            // TODO: Ekonomi haberleri listelenecek
            return View();
        }

        public IActionResult Dunya()
        {
            ViewData["Title"] = "Dünya Haberleri";
            return View();
        }

        public IActionResult Spor()
        {
            ViewData["Title"] = "Spor Haberleri";
            return View();
        }

        public IActionResult Kadin()
        {
            ViewData["Title"] = "Kadın Haberleri";
            return View();
        }

        public IActionResult Teknoloji()
        {
            ViewData["Title"] = "Teknoloji Haberleri";
            return View();
        }

        [HttpGet]
        public IActionResult AddCategoriesNow()
        {
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




        // --- SİSTEM & HESAP SAYFALARI ---

        public IActionResult Login()
        {
            ViewData["Title"] = "Giriş Yap";
            return View("~/Views/Account/Login.cshtml");
        }

        public IActionResult Register()
        {
            ViewData["Title"] = "Kayıt Ol";
            return View("~/Views/Account/Register.cshtml");
        }

        public IActionResult ForgotPassword()
        {
            ViewData["Title"] = "Şifremi Unuttum";
            return View("~/Views/Account/ForgotPassword.cshtml");
        }

        public IActionResult Page404()
        {
            ViewData["Title"] = "Sayfa Bulunamadı";
            return View("~/Views/Account/Page404.cshtml");
        }


        public IActionResult AddTestCategories()
        {
            try
            {


                return RedirectToAction("CreateNews");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Hata: {ex.Message}";
                return RedirectToAction("CreateNews");
            }
        }
    
   
        


    }
}
 