using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class AddReviewViewModel
    {
        public int PlayerId { get; set; } 

        [Required(ErrorMessage = "Your name is required.")]
        [StringLength(100)]
        [Display(Name = "Your Name")]
        public string ReviewerName { get; set; }

        [Required(ErrorMessage = "A comment is required.")]
        [StringLength(1000, ErrorMessage = "Comment cannot be longer than 1000 characters.")]
        [DataType(DataType.MultilineText)]
        public string CommentText { get; set; }

        [Required(ErrorMessage = "Please provide a rating.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        [Display(Name = "Rating (1-5 Stars)")]
        public int StarRating { get; set; }
    }
}