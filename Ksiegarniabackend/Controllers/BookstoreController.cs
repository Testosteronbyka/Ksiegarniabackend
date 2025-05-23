using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace Ksiegarniabackend.Controllers
{
    public class BookstoreController : Controller
    {
        private readonly HttpClient _httpClient;
        
        public BookstoreController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookService");
        }
        
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}