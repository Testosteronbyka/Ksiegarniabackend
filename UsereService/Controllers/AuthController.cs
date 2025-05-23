using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using UsereService.Models;

namespace UsereService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UsereDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UsereDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "Email i hasło są wymagane" });
                }

                var user = await _context.User.FirstOrDefaultAsync(
                    u => u.Email == request.Email && u.Password == request.Password && u.IsActive);

                if (user == null)
                {
                    _logger.LogWarning($"Nieudana próba logowania dla email: {request.Email}");
                    return Unauthorized(new { message = "Nieprawidłowy email lub hasło" });
                }

                // Aktualizacja czasu ostatniego logowania
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var userResponse = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    LastLoginAt = user.LastLoginAt
                };

                _logger.LogInformation($"Pomyślne logowanie użytkownika: {user.Email}");
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas logowania");
                return StatusCode(500, new { message = "Wystąpił błąd serwera" });
            }
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Username))
                {
                    return BadRequest(new { message = "Wszystkie pola są wymagane" });
                }

                // Sprawdź czy użytkownik już istnieje
                var existingUser = await _context.User.FirstOrDefaultAsync(
                    u => u.Email == request.Email || u.Username == request.Username);
                
                if (existingUser != null)
                {
                    return Conflict(new { message = "Użytkownik z tym emailem lub nazwą już istnieje" });
                }

                var newUser = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password, // W produkcji użyj hashowania haseł
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.User.Add(newUser);
                await _context.SaveChangesAsync();

                var userResponse = new UserResponse
                {
                    Id = newUser.Id,
                    Username = newUser.Username,
                    Email = newUser.Email,
                    LastLoginAt = null
                };

                return CreatedAtAction(nameof(Login), userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas rejestracji");
                return StatusCode(500, new { message = "Wystąpił błąd serwera" });
            }
        }

        // GET: api/auth/user/{id}
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _context.User.FindAsync(id);
                
                if (user == null || !user.IsActive)
                {
                    return NotFound(new { message = "Użytkownik nie został znaleziony" });
                }

                var userResponse = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    LastLoginAt = user.LastLoginAt
                };

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania użytkownika");
                return StatusCode(500, new { message = "Wystąpił błąd serwera" });
            }
        }
    }

    // DTO dla żądania logowania
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        public bool RememberMe { get; set; }
    }

    // DTO dla żądania rejestracji
    public class RegisterRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    // DTO dla odpowiedzi z danymi użytkownika
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? LastLoginAt { get; set; }
    }
}
