using System;
using System.Collections.Generic; // Required for ICollection
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models; // Assuming Review.cs is in WebApplication1.Models

public class Player
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string GamerTag { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public decimal PricePerHour { get; set; }

    [Range(0, 5)] // Overall player rating
    public double Rating { get; set; }

    // This is now a collection of Review objects
    public virtual ICollection<Review>? PlayerReviews { get; set; } = new List<Review>();

    [StringLength(2048)]
    public string? ImageUrl { get; set; }

    public int? TeamId { get; set; }
    [ForeignKey("TeamId")]
    public virtual Team? Team { get; set; }
}