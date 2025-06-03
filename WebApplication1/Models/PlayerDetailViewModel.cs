using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class PlayerDetailViewModel
    {
        public Player Player { get; set; }
        public List<Review> ReviewsOnCurrentPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public PlayerDetailViewModel()
        {
            ReviewsOnCurrentPage = new List<Review>();
        }
    }
}