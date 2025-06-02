using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

        builder.Entity<Player>()
            .HasOne(p => p.ApplicationUser)
            .WithOne() // Assuming one Player per ApplicationUser
            .HasForeignKey<Player>(p => p.ApplicationUserId)
            .IsRequired();

        builder.Entity<Team>()
            .HasMany(t => t.Players)
            .WithOne(p => p.Team)
            .HasForeignKey(p => p.TeamId)
            .IsRequired(false);

        // Seed Data
        string ADMIN_ROLE_ID = Guid.NewGuid().ToString();
        string ADMIN_USER_ID = Guid.NewGuid().ToString();

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
            PasswordHash = hasher.HashPassword(null, "Admin123!"), // Replace with a strong password
            SecurityStamp = Guid.NewGuid().ToString("D")
        });

        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = ADMIN_ROLE_ID,
            UserId = ADMIN_USER_ID
        });

        builder.Entity<Team>().HasData(
            new Team { Id = 1, Name = "Team Alpha", Description = "The first team", DateCreated = DateTime.UtcNow },
            new Team { Id = 2, Name = "Team Bravo", Description = "The second team", DateCreated = DateTime.UtcNow }
        );

        builder.Entity<Player>().HasData(
            new Player
            {
                Id = 1,
                GamerTag = "AdminPlayer",
                Description = "The admin's player profile",
                Email = "admin@example.com",
                RegistrationDate = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow,
                Level = 10,
                ExperiencePoints = 10000,
                VirtualCurrencyBalance = 500.00m,
                ApplicationUserId = ADMIN_USER_ID, // Link to the admin ApplicationUser
                TeamId = 1
            },
            new Player
            {
                Id = 2,
                GamerTag = "HeroPlayer",
                Description = "A heroic player",
                Email = "hero@example.com", // This player will need a corresponding ApplicationUser if they need to log in
                RegistrationDate = DateTime.UtcNow,
                Level = 5,
                ExperiencePoints = 2500,
                VirtualCurrencyBalance = 100.00m,
                ApplicationUserId = Guid.NewGuid().ToString(), // Placeholder: Create a real ApplicationUser for this player if needed
                TeamId = 2
            }
        );
    }
}