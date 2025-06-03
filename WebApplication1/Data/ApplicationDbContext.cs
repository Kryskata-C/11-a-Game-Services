using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using WebApplication1.Models; // Ensure this namespace is correct for Player and Review

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Player>(entity =>
        {
            entity.HasIndex(p => p.GamerTag).IsUnique();
            entity.HasMany(p => p.PlayerReviews)
                  .WithOne(r => r.Player)
                  .HasForeignKey(r => r.PlayerId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(p => p.PricePerHour).HasColumnType("decimal(18,2)");
        });

        builder.Entity<Team>()
            .HasMany(t => t.Players)
            .WithOne(p => p.Team)
            .HasForeignKey(p => p.TeamId)
            .IsRequired(false);

        // Seed Data
        // **USE THESE FIXED GUIDs or generate your own ONCE and keep them**
        const string ADMIN_ROLE_ID = "a1b2c3d4-e5f6-7777-8888-9999abcdefff"; // Example Fixed GUID
        const string ADMIN_USER_ID = "f9e8d7c6-b5a4-3333-2222-1111fedcba98"; // Example Fixed GUID

        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Id = ADMIN_ROLE_ID, // Use fixed ID
            Name = "Admin",
            NormalizedName = "ADMIN"
        });

        var hasher = new PasswordHasher<ApplicationUser>();
        builder.Entity<ApplicationUser>().HasData(new ApplicationUser
        {
            Id = ADMIN_USER_ID, // Use fixed ID
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "P@$$wOrd123"), // Use a strong, unique password
            SecurityStamp = "ABCDEF01-2345-6789-ABCD-EF0123456789" // Fixed SecurityStamp for seeded user
        });

        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = ADMIN_ROLE_ID, // Use fixed ID
            UserId = ADMIN_USER_ID  // Use fixed ID
        });

        builder.Entity<Team>().HasData(
            new Team { Id = 1, Name = "Team Alpha", Description = "The first team", DateCreated = DateTime.UtcNow },
            new Team { Id = 2, Name = "Team Bravo", Description = "The second team", DateCreated = DateTime.UtcNow }
        );

        builder.Entity<Player>().HasData(
            new Player
            {
                Id = 1,
                GamerTag = "ProGamerX",
                Description = "Top tier player...",
                PricePerHour = 50.00m,
                Rating = 4.8,
                TeamId = 1,
                ImageUrl = "https://example.com/images/progamerx.png"
            },
            new Player
            {
                Id = 2,
                GamerTag = "StrategistSupreme",
                Description = "Specializes in strategy...",
                PricePerHour = 40.00m,
                Rating = 4.5,
                TeamId = 2,
                ImageUrl = "https://example.com/images/strategistsupreme.png"
            },
            new Player
            {
                Id = 3,
                GamerTag = "NewTalent",
                Description = "Up and coming player...",
                PricePerHour = 20.00m,
                Rating = 3.9,
                TeamId = 1,
                ImageUrl = "https://example.com/images/newtalent.png"
            }
        );

        builder.Entity<Review>().HasData(
            new Review
            {
                Id = 1,
                PlayerId = 1,
                ReviewerName = "UserA",
                CommentText = "Excellent communication and skill for ProGamerX!",
                ReviewDate = DateTime.UtcNow.AddDays(-5),
                StarRating = 5
            },
            new Review
            {
                Id = 2,
                PlayerId = 1,
                ReviewerName = "UserB",
                CommentText = "Very professional (ProGamerX).",
                ReviewDate = DateTime.UtcNow.AddDays(-2),
                StarRating = 4
            },
            new Review
            {
                Id = 3,
                PlayerId = 2,
                ReviewerName = "UserC",
                CommentText = "Great tactical mind (StrategistSupreme)!",
                ReviewDate = DateTime.UtcNow.AddDays(-3),
                StarRating = 5
            }
        );
    }
}