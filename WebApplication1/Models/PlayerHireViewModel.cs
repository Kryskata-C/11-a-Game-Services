using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class PlayerHireViewModel
    {
        public IEnumerable<Player> Players { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public string CurrentSortOrder { get; set; }
        public string GamerTagSortParam { get; set; }
        public string PriceSortParam { get; set; }
        public string RatingSortParam { get; set; }
        public string CurrentSearchTerm { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PlayerHireViewModel()
        {
            Players = new List<Player>();
        }
    }
}