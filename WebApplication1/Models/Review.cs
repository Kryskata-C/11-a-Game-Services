using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models // Ensure this namespace matches your project
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string CommentText { get; set; } // Renamed from ReviewText

        public DateTime ReviewDate { get; set; }

        [Required]
        public string ReviewerName { get; set; }

        public int StarRating { get; set; } // Renamed from Rating (e.g., 1-5 stars)

        // Foreign key for Player
        public int PlayerId { get; set; }
        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }
    }
}