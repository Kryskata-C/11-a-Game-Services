using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Required for Column attribute

public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")] // Store as decimal in DB
    public decimal Balance { get; set; } = 0.00m; // Default balance to 0
}