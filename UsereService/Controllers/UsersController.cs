using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UsereService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UseresController : ControllerBase
    {
        private readonly UsereDbContext _context;

        public UseresController(UsereDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usere>>> GetUsers()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usere>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Nie zwracaj hasła w odpowiedzi API
            user.Password = null;

            return user;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<Usere>> CreateUser(Usere usere)
        {
            // W rzeczywistej aplikacji trzeba zahashować hasło!
            _context.User.Add(usere);
            await _context.SaveChangesAsync();

            // Nie zwracaj hasła w odpowiedzi API
            usere.Password = null;

            return CreatedAtAction(nameof(GetUser), new { id = usere.Id }, usere);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, Usere usere)
        {
            if (id != usere.Id)
            {
                return BadRequest();
            }

            _context.Entry(usere).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
