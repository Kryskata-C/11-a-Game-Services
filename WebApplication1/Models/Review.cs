using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data.Entities;

namespace WebApplication1.Models 
{
    public class Review
    {
        public int Id { get; set; }

        public int? PlayerId { get; set; } 
        [ForeignKey("PlayerId")]
        public virtual Player? Player { get; set; } 

        public int? TeamId { get; set; } 
        [ForeignKey("TeamId")]
        public virtual Team? Team { get; set; } 

        public string? UserId { get; set; } 
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Reviewer name cannot be empty.")]
        [StringLength(100)]
        [Display(Name = "Your Name")]
        public string ReviewerName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Comment cannot be empty.")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Comment must be between 5 and 1000 characters.")]
        [Display(Name = "Your Review")]
        public string CommentText { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        [Display(Name = "Star Rating")]
        public int StarRating { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;


    }
}