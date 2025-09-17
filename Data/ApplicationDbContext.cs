using GraduationQRSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace GraduationQRSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Senior> Seniors => Set<Senior>();
        public DbSet<Guest> Guests => Set<Guest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Senior>()
                .HasMany(s => s.Guests)
                .WithOne(g => g.Senior!)
                .HasForeignKey(g => g.SeniorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Senior>()
                .Property(s => s.Name)
                .IsRequired();

            modelBuilder.Entity<Guest>()
                .Property(g => g.Name)
                .IsRequired();
        }
    }
}


