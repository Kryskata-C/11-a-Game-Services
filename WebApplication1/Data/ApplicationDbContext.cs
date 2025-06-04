using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using WebApplication1.Data.Entities;
using WebApplication1.Models;

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
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(p => p.PricePerHour).HasColumnType("decimal(18,2)");
        });

        builder.Entity<Team>(entity =>
        {
            entity.HasMany(t => t.Players)
                  .WithOne(p => p.Team)
                  .HasForeignKey(p => p.TeamId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(t => t.Reviews)
                  .WithOne(r => r.Team)
                  .HasForeignKey(r => r.TeamId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(t => t.PricePerHour).HasColumnType("decimal(18,2)");
        });

        builder.Entity<Review>(entity =>
        {
            entity.HasOne(r => r.Player)
                  .WithMany(p => p.PlayerReviews)
                  .HasForeignKey(r => r.PlayerId)
                  .IsRequired(false);

            entity.HasOne(r => r.Team)
                  .WithMany(t => t.Reviews)
                  .HasForeignKey(r => r.TeamId)
                  .IsRequired(false);
        });

        const string ADMIN_ROLE_ID = "a1b2c3d4-e5f6-7777-8888-9999abcdefff";
        const string ADMIN_USER_ID = "f9e8d7c6-b5a4-3333-2222-1111fedcba98";

        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Id = ADMIN_ROLE_ID,
            Name = "Admin",
            NormalizedName = "ADMIN"
        });

        var hasher = new PasswordHasher<ApplicationUser>();
        builder.Entity<ApplicationUser>().HasData(new ApplicationUser
        {
            Id = ADMIN_USER_ID,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "P@$$wOrd123"),
            SecurityStamp = "ABCDEF01-2345-6789-ABCD-EF0123456789"
        });

        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = ADMIN_ROLE_ID,
            UserId = ADMIN_USER_ID
        });

        builder.Entity<Team>().HasData(
            new Team { Id = 1, Name = "Team Alpha", Description = "The first team", PricePerHour = 100.00m, Rating = 0, DateCreated = DateTime.UtcNow, ImageUrl = "/images/default-team.png" },
            new Team { Id = 2, Name = "Team Bravo", Description = "The second team", PricePerHour = 120.00m, Rating = 0, DateCreated = DateTime.UtcNow, ImageUrl = "/images/default-team.png" }
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
                ImageUrl = "/images/playeruploads/default-player.png"
            },
            new Player
            {
                Id = 2,
                GamerTag = "StrategistSupreme",
                Description = "Specializes in strategy...",
                PricePerHour = 40.00m,
                Rating = 4.5,
                TeamId = 2,
                ImageUrl = "/images/playeruploads/default-player.png"
            },
            new Player
            {
                Id = 3,
                GamerTag = "NewTalent",
                Description = "Up and coming player...",
                PricePerHour = 20.00m,
                Rating = 3.9,
                TeamId = 1,
                ImageUrl = "/images/playeruploads/default-player.png"
            },
            new Player
            {
                Id = 4,
                GamerTag = "SoloStar",
                Description = "Prefers to work alone, but available.",
                PricePerHour = 30.00m,
                Rating = 4.2,
                TeamId = null,
                ImageUrl = "/images/playeruploads/default-player.png"
            }
        );

        builder.Entity<Review>().HasData(
            new Review
            {
                Id = 1,
                PlayerId = 1,
                TeamId = null,
                ReviewerName = "UserA",
                CommentText = "Excellent communication and skill for ProGamerX!",
                ReviewDate = DateTime.UtcNow.AddDays(-5),
                StarRating = 5
            },
            new Review
            {
                Id = 2,
                PlayerId = 1,
                TeamId = null,
                ReviewerName = "UserB",
                CommentText = "Very professional (ProGamerX).",
                ReviewDate = DateTime.UtcNow.AddDays(-2),
                StarRating = 4
            },
            new Review
            {
                Id = 3,
                PlayerId = 2,
                TeamId = null,
                ReviewerName = "UserC",
                CommentText = "Great tactical mind (StrategistSupreme)!",
                ReviewDate = DateTime.UtcNow.AddDays(-3),
                StarRating = 5
            }
        );
    }
}