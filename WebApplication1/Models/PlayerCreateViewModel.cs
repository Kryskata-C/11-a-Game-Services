using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models 
{
    public class PlayerCreateViewModel
    {
        [Required(ErrorMessage = "GamerTag is required.")]
        [StringLength(100)]
        public string GamerTag { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price per hour is required.")]
        [DataType(DataType.Currency)]
        [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0.")] 
        public decimal PricePerHour { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public double Rating { get; set; }

        public string Reviews { get; set; } 

        [Display(Name = "Player Image")]
        public IFormFile ImageFile { get; set; }
    }
}