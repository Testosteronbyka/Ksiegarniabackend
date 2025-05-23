using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ksiegarniabackend.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IHttpClientFactory httpClientFactory,
            ILogger<AccountController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Użyj klienta HTTP dla UsereService
                    var client = _httpClientFactory.CreateClient("UserService");
                    
                    // Przygotowanie danych do wysłania
                    var loginData = new 
                    {
                        Email = model.Email,
                        Password = model.Password,
                        RememberMe = model.RememberMe
                    };
                    
                    // Serializacja danych do formatu JSON
                    var content = new StringContent(
                        JsonSerializer.Serialize(loginData),
                        Encoding.UTF8,
                        "application/json");

                    // Wywołanie endpointu logowania w UsereService
                    var response = await client.PostAsync("api/auth/login", content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var userData = await response.Content.ReadAsStringAsync();
                        var user = JsonSerializer.Deserialize<UserDto>(userData, 
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        
                        // Zapisanie danych użytkownika w sesji
                        HttpContext.Session.SetString("UserId", user.Id.ToString());
                        HttpContext.Session.SetString("UserName", user.Username);
                        HttpContext.Session.SetString("IsAuthenticated", "true");
                        
                        return Json(new { success = true });
                    }
                    
                    _logger.LogWarning($"Nieudane logowanie dla użytkownika {model.Email}");
                    return Json(new { success = false, error = "Niepoprawne dane logowania" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Błąd podczas logowania");
                    return Json(new { success = false, error = ex.Message });
                }
            }
            
            return Json(new { success = false, error = "Niepoprawne dane formularza" });
        }

        // POST: /Account/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }

    // Model widoku logowania
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    // DTO dla odpowiedzi z API
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
