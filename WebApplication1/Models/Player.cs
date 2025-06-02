using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Player
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string GamerTag { get; set; } // This can stay as the player's professional name/tag

    [StringLength(1000)] // Increased length for more detailed descriptions or multiple reviews
    public string Description { get; set; } // General description of the player/service

    public decimal PricePerHour { get; set; } // Cost to hire the player

    [Range(0, 5)] // Assuming a 0-5 star rating system
    public double Rating { get; set; }

    public string Reviews { get; set; } // Could be a simple text field for reviews, or a JSON string, or link to a review system

    // Team association can remain if players for hire can be part of a team
    public int? TeamId { get; set; }
    [ForeignKey("TeamId")]
    public virtual Team Team { get; set; }
    public string ImageUrl { get; set; }
    // Properties to remove:
    // public string Email { get; set; } // Users have emails, not the player products
    // public DateTime RegistrationDate { get; set; } // Not relevant for a product
    // public DateTime? LastLoginDate { get; set; } // Not relevant for a product
    // public int Level { get; set; } // This might be relevant depending on the game/service, but let's simplify for now
    // public long ExperiencePoints { get; set; } // As above
    // public decimal VirtualCurrencyBalance { get; set; } // Not relevant for a hireable product
    // public string ApplicationUserId { get; set; } // CRITICAL: This is the main link to User that needs to be severed
    // public virtual ApplicationUser ApplicationUser { get; set; } // Corresponding navigation property
}