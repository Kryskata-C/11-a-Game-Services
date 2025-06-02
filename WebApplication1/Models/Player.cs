using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Player
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string GamerTag { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public DateTime RegistrationDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public int Level { get; set; }
    public long ExperiencePoints { get; set; }
    public decimal VirtualCurrencyBalance { get; set; }

    [Required]
    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    public virtual ApplicationUser ApplicationUser { get; set; }

    public int? TeamId { get; set; } 
    [ForeignKey("TeamId")]
    public virtual Team Team { get; set; } 
}
