using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Data.Entities; 

namespace WebApplication1.Models
{
    public class TeamEditViewModel
    {
        public int Id { get; set; } 

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [DataType(DataType.Currency)]
        [Range(0, 10000.00, ErrorMessage = "Price per hour must be between 0 and 10,000.")]
        public decimal PricePerHour { get; set; }

        public string? ExistingImageUrl { get; set; } 

        [Display(Name = "New Team Image (Optional)")]
        public IFormFile? NewTeamImageFile { get; set; } 

        [Display(Name = "Players")]
        public List<int>? SelectedPlayerIds { get; set; } 

        public List<SelectListItem>? AvailablePlayers { get; set; } 

        public TeamEditViewModel()
        {
            SelectedPlayerIds = new List<int>();
            AvailablePlayers = new List<SelectListItem>();
        }
    }
}