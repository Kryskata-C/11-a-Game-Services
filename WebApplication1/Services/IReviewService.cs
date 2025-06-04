using System.Threading.Tasks;
using WebApplication1.Models; 
using System.Security.Claims; 

namespace WebApplication1.Services
{
    public interface IReviewService
    {
        Task<Review> GetReviewByIdAsync(int id);
        Task<Review> GetReviewWithPlayerAndTeamAsync(int id); 
        Task AddPlayerReviewAsync(AddReviewViewModel model, string userId, string reviewerName);
        Task<bool> UpdateReviewAsync(EditReviewViewModel model, string currentUserId, bool isAdmin);
        Task<(bool Success, Review ReviewToDelete)> CanDeleteReviewAsync(int id, string currentUserId, bool isAdmin);
        Task DeleteReviewAsync(int id);
        Task<bool> ReviewExistsAsync(int id);
        Task<Player> GetPlayerForReviewAsync(int playerId); 
    }
}