using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models 
{
    public class ReviewSubViewModel
    {
        [Required(ErrorMessage = "Reviewer name is required.")]
        [StringLength(100)]
        public string ReviewerName { get; set; }

        [Required(ErrorMessage = "A comment is required.")]
        [StringLength(1000)]
        public string CommentText { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int StarRating { get; set; }
    }

    public class PlayerCreateViewModel
    {
        // ... other properties ...
        public string GamerTag { get; set; } //
        public string Description { get; set; } //
        public decimal PricePerHour { get; set; } //
        public double Rating { get; set; } //
                                           // Remove: public string Reviews { get; set; }
        public IFormFile ImageFile { get; set; } //

        public List<ReviewSubViewModel> InitialReviews { get; set; } = new List<ReviewSubViewModel>();
    }
}