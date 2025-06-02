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
            .WithOne()
            .HasForeignKey<Player>(p => p.ApplicationUserId)
            .IsRequired();

        builder.Entity<Team>()
            .HasMany(t => t.Players)       
            .WithOne(p => p.Team)          
            .HasForeignKey(p => p.TeamId)  
            .IsRequired(false);            
                                           
    }
}