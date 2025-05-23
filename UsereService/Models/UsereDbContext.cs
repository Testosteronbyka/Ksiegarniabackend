using Microsoft.EntityFrameworkCore;

namespace UsereService.Models
{
    public class UsereDbContext : DbContext
    {
        public UsereDbContext(DbContextOptions<UsereDbContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
                
                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
                    
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);
                    
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}