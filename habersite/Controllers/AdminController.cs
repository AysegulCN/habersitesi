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
            ViewData["Title"] = "Yeni Haber Ekle";

            try
            {
                // Kategorileri getir - null kontrolü ekle
                var categoriesList = _context.Categories
                    .Where(c => c.IsActive)
                    .ToList();

                ViewBag.Categories = categoriesList ?? new List<Category>();
            }
            catch (Exception ex)
            {
                // Hata durumunda boş liste
                ViewBag.Categories = new List<Category>();
                // Hata mesajını loglayabilirsiniz
                Console.WriteLine($"Kategori getirme hatası: {ex.Message}");
            }

            return View();
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
        public IActionResult AddCategoriesNow()
        {
            // Direkt SQL çalıştır
              _context.Database.ExecuteSqlRaw(@"
          IF NOT EXISTS (SELECT * FROM Categories WHERE Name = 'Ekonomi')
          BEGIN
            INSERT INTO Categories (Name, Description, IsActive) VALUES
            ('Ekonomi', 'Ekonomi haberleri', 1),
            ('Dünya', 'Dünya haberleri', 1),
            ('Spor', 'Spor haberleri', 1),
            ('Kadın', 'Kadın haberleri', 1),
            ('Teknoloji', 'Teknoloji haberleri', 1);
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
 