using Microsoft.AspNetCore.Mvc;

namespace Ksiegarniabackend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); 
        }
        // Akcja do wyświetlania historii firmy
        public IActionResult About()
        {
            return View();
        }

        // Akcja do wyświetlania kontaktu
        public IActionResult Contact()
        {
            return View();
        }
    }
}