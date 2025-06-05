using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class DepositViewModel
    {
        [Required]
        [DataType(DataType.Currency)]
        [Range(0.01, 10000.00, ErrorMessage = "Deposit amount must be between {1:C} and {2:C}.")]
        [Display(Name = "Amount to Deposit")]
        public decimal Amount { get; set; }
    }
}