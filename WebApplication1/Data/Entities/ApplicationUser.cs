using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    // Add any custom properties for your application users here
    // For example:
    // [StringLength(100)]
    // public string? FirstName { get; set; }

    // [StringLength(100)]
    // public string? LastName { get; set; }

    // Navigation property back to Player (if you want a direct link from User to Player profile)
    // This assumes one-to-one or one-to-zero/one between ApplicationUser and Player
    // public virtual Player? PlayerProfile { get; set; }
}   