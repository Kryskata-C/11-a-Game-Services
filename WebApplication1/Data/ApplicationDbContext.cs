// File: WebApplication1/Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models; // Assuming Player, Team, Review, ApplicationUser are accessible via this or other relevant using statements

namespace WebApplication1.Data // Or your actual data context namespace
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Review> Reviews { get; set; }

        // Corrected OnModelCreating method
        protected override void OnModelCreating(ModelBuilder modelBuilder) // Only one parameter: ModelBuilder
        {
            base.OnModelCreating(modelBuilder); // Correctly calls the base method with one parameter

            modelBuilder.Entity<Player>(entity =>
            {
                entity.Property(p => p.PricePerHour).HasColumnType("decimal(18, 2)");

            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasOne(r => r.Player)
                    .WithMany(p => p.PlayerReviews)
                    .HasForeignKey(r => r.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade); 

            });

        }
    }
}