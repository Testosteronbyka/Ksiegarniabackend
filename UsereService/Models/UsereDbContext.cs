using Microsoft.EntityFrameworkCore;

namespace UserService.Models
{
    public class UsereDbContext : DbContext
    {
        public UsereDbContext(DbContextOptions<UsereDbContext> options) : base(options)
        {
        }

        public DbSet<Usere> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Konfiguracja modelu
            modelBuilder.Entity<Usere>()
                .HasKey(u => u.Id);
                
            modelBuilder.Entity<Usere>()
                .HasIndex(u => u.Email)
                .IsUnique();
                
            modelBuilder.Entity<Usere>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Przykładowe dane
            modelBuilder.Entity<Usere>().HasData(
                new Usere { 
                    Id = 1, 
                    Username = "jan_kowalski", 
                    Email = "jan.kowalski@example.com", 
                    Password = "Test123!", 
                    FullName = "Jan Kowalski", 
                    Address = "ul. Przykładowa 1, 00-001 Warszawa", 
                    Phone = "123456789" 
                }
            );
        }
    }
}