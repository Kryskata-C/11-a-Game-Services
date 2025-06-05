using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class CheckoutViewModel
    {
        public List<CheckoutItemViewModel> Items { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal UserBalance { get; set; }
        public bool CanAfford => UserBalance >= OrderTotal;

        public CheckoutViewModel()
        {
            Items = new List<CheckoutItemViewModel>();
        }
    }

    public class CheckoutItemViewModel
    {
        public string ServiceName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
    }
} 