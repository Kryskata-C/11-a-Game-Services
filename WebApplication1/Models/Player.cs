// File: WebApplication1/Models/Player.cs
using System;
using System.Collections.Generic; // Required for ICollection
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models;
public class Player
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string GamerTag { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public decimal PricePerHour { get; set; }

        [Range(0, 5)]
        public double Rating { get; set; }

        // REMOVE the old string Reviews property:
        // public string Reviews { get; set; } 

        // ADD this collection for structured reviews:
        public virtual ICollection<Review> PlayerReviews { get; set; } // Changed name to avoid confusion

        public int? TeamId { get; set; }
        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }

        public string ImageUrl { get; set; }

        public Player()
        {
            PlayerReviews = new HashSet<Review>(); // Initialize the collection
        }
    
}