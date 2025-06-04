using System.Collections.Generic;
using WebApplication1.Data.Entities; // Required for Team

namespace WebApplication1.Models
{
    public class TeamIndexViewModel
    {
        public IEnumerable<Team> Teams { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public string CurrentSortOrder { get; set; }
        public string NameSortParam { get; set; }
        public string PriceSortParam { get; set; }
        public string RatingSortParam { get; set; }
        public string DateCreatedSortParam { get; set; }
        public string CurrentSearchTerm { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public TeamIndexViewModel()
        {
            Teams = new List<Team>();
        }
    }
}