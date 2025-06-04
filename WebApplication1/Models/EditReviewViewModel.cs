using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class EditReviewViewModel
    {
        public int Id { get; set; } 

        public int? PlayerId { get; set; }
        public string? PlayerName { get; set; } 
        public int? TeamId { get; set; }
        public string? TeamName { get; set; } 


        [Display(Name = "Reviewer")]
        public string ReviewerName { get; set; } 

        [Required(AllowEmptyStrings = false, ErrorMessage = "Comment cannot be empty.")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Comment must be between 5 and 1000 characters.")]
        [Display(Name = "Your Review")]
        public string CommentText { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        [Display(Name = "Star Rating")]
        public int StarRating { get; set; }

        public string? OriginalUserId { get; set; }
    }
}