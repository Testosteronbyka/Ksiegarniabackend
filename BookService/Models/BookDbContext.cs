using Microsoft.EntityFrameworkCore;

namespace BookService.Models
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Konfiguracja modelu
            modelBuilder.Entity<Book>()
                .HasKey(b => b.Id);

            // Przykładowe dane
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Pan Tadeusz", Author = "Adam Mickiewicz", ISBN = "9788308063231", Price = 29.99m, Quantity = 10, Description = "Epopeja narodowa" },
                new Book { Id = 2, Title = "Lalka", Author = "Bolesław Prus", ISBN = "9788308063248", Price = 34.99m, Quantity = 5, Description = "Powieść społeczno-obyczajowa" }
            );
        }
    }
}