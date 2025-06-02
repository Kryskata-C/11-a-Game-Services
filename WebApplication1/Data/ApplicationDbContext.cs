using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System; // Ensure System is imported for DateTime

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players { get; set; }
    public DbSet<Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Player>()
            .HasIndex(p => p.GamerTag)
            .IsUnique();

        // REMOVE THIS: Player is no longer directly linked to an ApplicationUser in a one-to-one required manner
        // builder.Entity<Player>()
        //     .HasOne(p => p.ApplicationUser)
        //     .WithOne() 
        //     .HasForeignKey<Player>(p => p.ApplicationUserId)
        //     .IsRequired();

        builder.Entity<Team>()
            .HasMany(t => t.Players)
            .WithOne(p => p.Team)
            .HasForeignKey(p => p.TeamId)
            .IsRequired(false); // Keep this if players can belong to teams

        // Seed Data
        string ADMIN_ROLE_ID = Guid.NewGuid().ToString();
        // string ADMIN_USER_ID = Guid.NewGuid().ToString(); // This ID is for the ApplicationUser (admin)

        // Keep Admin User and Role seeding as is
        var adminUserId = Guid.NewGuid().ToString(); // Use a local var for clarity if preferred
        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Id = ADMIN_ROLE_ID,
            Name = "Admin",
            NormalizedName = "ADMIN"
        });

        var hasher = new PasswordHasher<ApplicationUser>();
        builder.Entity<ApplicationUser>().HasData(new ApplicationUser
        {
            Id = adminUserId, // Use the generated ID
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "Admin123!"),
            SecurityStamp = Guid.NewGuid().ToString("D")
        });

        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = ADMIN_ROLE_ID,
            UserId = adminUserId // Use the generated ID
        });


        builder.Entity<Team>().HasData(
            new Team { Id = 1, Name = "Team Alpha", Description = "The first team", DateCreated = DateTime.UtcNow },
            new Team { Id = 2, Name = "Team Bravo", Description = "The second team", DateCreated = DateTime.UtcNow }
        );

        // Updated Player Seed Data
        // Inside OnModelCreating method in ApplicationDbContext.cs

        // ... (other seed data like Teams, Admin User, etc.)

        builder.Entity<Player>().HasData(
            new Player
            {
                Id = 1,
                GamerTag = "ProGamerX", // Or your existing GamerTag
                Description = "Top tier player with 5 years of competitive experience.", // Or your existing Description
                PricePerHour = 50.00m,
                Rating = 4.8,
                Reviews = "Excellent communication and skill.",
                TeamId = 1,
                ImageUrl = "https://example.com/images/progamerx.png" // << ADD THIS LINE (or a placeholder URL)
            },
            new Player
            {
                Id = 2,
                GamerTag = "StrategistSupreme", // Or your existing GamerTag
                Description = "Specializes in strategy and team coordination.", // Or your existing Description
                PricePerHour = 40.00m,
                Rating = 4.5,
                Reviews = "Great tactical mind, helped our team immensely.",
                TeamId = 2,
                ImageUrl = "https://example.com/images/strategistsupreme.png" // << ADD THIS LINE
            },
            new Player // If you have a third player or more, update them too
            {
                Id = 3,
                GamerTag = "NewTalent", // Or your existing GamerTag
                Description = "Up and coming player, eager to learn and assist.", // Or your existing Description
                PricePerHour = 20.00m,
                Rating = 3.9,
                Reviews = "Good potential, friendly and responsive.",
                TeamId = 1,
                ImageUrl = "https://example.com/images/newtalent.png" // << ADD THIS LINE
            }
            // Add ImageUrl = "some_url_or_empty_string_if_allowed_and_configured_as_nullable"
            // for ALL Player entities you are seeding.
        );
    }
}