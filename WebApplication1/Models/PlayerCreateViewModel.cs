using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // Required for ValidateNever

namespace WebApplication1.Models
{
    public class PlayerCreateViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string GamerTag { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, 10000.00)]
        [DataType(DataType.Currency)]
        public decimal PricePerHour { get; set; }

        [Range(0.0, 5.0)] // This is the player's overall self-assigned/initial rating
        public double Rating { get; set; } = 0.0;

        [Display(Name = "Player Image")]
        [ValidateNever] // Allows for optional image upload or different validation logic
        public IFormFile? ImageFile { get; set; }

        // This is the new property for initial reviews
        [Display(Name = "Initial Reviews")]
        [ValidateNever] // We'll validate individual review items if submitted
        public List<AddReviewViewModel> InitialReviews { get; set; } = new List<AddReviewViewModel>();
    }
}