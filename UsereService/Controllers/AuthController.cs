using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UsereService.Models;

namespace UsereService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UsereDbContext _context;

        public AuthController(UsereDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email i hasło są wymagane");
            }

            // Znajdź użytkownika w bazie danych
            var user = await _context.User.FirstOrDefaultAsync(
                u => u.Email == request.Email && u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized("Nieprawidłowy email lub hasło");
            }

            // Zwróć dane użytkownika bez hasła
            var userResponse = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return Ok(userResponse);
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email i hasło są wymagane");
            }

            // Sprawdź czy użytkownik już istnieje
            var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return Conflict("Użytkownik z tym emailem już istnieje");
            }

            // Utwórz nowego użytkownika
            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password // W produkcji należy zahashować hasło
            };

            _context.User.Add(newUser);
            await _context.SaveChangesAsync();

            var userResponse = new UserResponse
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Email = newUser.Email
            };

            return CreatedAtAction(nameof(Login), userResponse);
        }
    }

    // DTO dla żądania logowania
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    // DTO dla żądania rejestracji
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // DTO dla odpowiedzi z danymi użytkownika
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
