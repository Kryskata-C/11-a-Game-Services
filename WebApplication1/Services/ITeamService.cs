using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Data.Entities; 
using WebApplication1.Models; 

namespace WebApplication1.Services
{
    public interface ITeamService
    {
        Task<(IEnumerable<Team> Teams, int TotalCount)> GetTeamsAsync(string searchTerm, string sortOrder, int pageNumber, int pageSize);
        Task<Team> GetTeamByIdAsync(int id);
        Task<Team> GetTeamWithDetailsAsync(int id); 
        Task CreateTeamAsync(Team team, List<int> selectedPlayerIds, string currentUserId, IEnumerable<AddReviewViewModel> initialTeamReviews, string webRootPath, string teamImageFileName);
        Task<bool> UpdateTeamAsync(TeamEditViewModel model, string webRootPath);
        Task DeleteTeamAsync(int id, string webRootPath);
        Task<bool> TeamExistsAsync(int id);
        Task<List<SelectListItem>> GetAvailablePlayersForCreateAsync(List<int> alreadySelectedPlayerIds);
        Task<List<SelectListItem>> GetAvailablePlayersForEditAsync(int teamId, List<int> currentSelectedPlayerIdsOnForm);
    }
}