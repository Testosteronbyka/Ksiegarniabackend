using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ksiegarniabackend.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var client = _httpClientFactory.CreateClient("UserService");

            // Pobierz listę użytkowników z mikroserwisu
            var response = await client.GetAsync("/");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Błąd serwera podczas próby logowania: {response.StatusCode}";
                return View();
            }

            var usersJson = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<UserDto>>(usersJson);

            // Znajdź użytkownika z odpowiednimi danymi logowania
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                ViewBag.Error = "Niepoprawne dane logowania.";
                return View();
            }

            // Ustawienie sesji dla zalogowanego użytkownika
            HttpContext.Session.SetString("User", user.Username);

            // Przekierowanie na stronę główną po udanym logowaniu
            return RedirectToAction("Index", "Home");
        }

        // Wylogowanie użytkownika
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

    // Tymczasowa klasa DTO do mapowania zwrotu z mikroserwisu
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}