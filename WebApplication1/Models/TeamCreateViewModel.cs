using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; 

namespace WebApplication1.Models
{
    public class TeamCreateViewModel
    {
        [Required(ErrorMessage = "Team name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Team name must be between 3 and 100 characters.")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price per hour is required.")]
        [DataType(DataType.Currency)]
        [Range(0.01, 100000.00, ErrorMessage = "Price must be a positive value (e.g., 0.01 to 100,000.00).")]
        [Display(Name = "Price Per Hour")]
        public decimal PricePerHour { get; set; }

        [Display(Name = "Team Logo/Image")]
        [ValidateNever] 
        public IFormFile? TeamImageFile { get; set; }

        [Display(Name = "Team Members")]
        public List<int> SelectedPlayerIds { get; set; } = new List<int>();

        [ValidateNever]
        public List<SelectListItem> AvailablePlayers { get; set; } = new List<SelectListItem>();

        [Display(Name = "Initial Team Reviews")]
        [ValidateNever] 
        public List<AddReviewViewModel> InitialTeamReviews { get; set; } = new List<AddReviewViewModel>();
    }
}