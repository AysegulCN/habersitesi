using Microsoft.AspNetCore.Mvc;

namespace habersite.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Yönetim Paneli Ana Sayfa";
            return View();
        }

        public IActionResult NewsList()
        {
            ViewData["Title"] = "Haber Listesi"; 
            return View(); 
        }
        public IActionResult CategoryList()
        {
            ViewData["Title"] = "Kategori Yönetimi";
            return View();
        }

        public IActionResult Raporlar()
        {
            // Bu, sayfanın başlığını belirler.
            ViewData["Title"] = "Site İstatistikleri ve Raporlar";

            // Bu kod, Views/Admin/Charts.cshtml dosyasını arar ve yükler.
            return View();
        }
        public IActionResult Ekonomi()
        {
            ViewData["Title"] = "Ekonomi Haberleri";
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
            ViewData["Title"] = "Sayfa Bulunamadı ";
            return View("~/Views/Account/Page404.cshtml");
        }



    }
}