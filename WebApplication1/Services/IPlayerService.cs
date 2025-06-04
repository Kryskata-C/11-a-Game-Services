// using WebApplication1.Models; // Ensure Player model is accessible
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public interface IPlayerService
    {
        Task<(IEnumerable<Player> Players, int TotalCount)> GetAllPlayersAsync(
        string searchTerm,
        string sortOrder,
        int pageNumber,
        int pageSize);
        Task<Player> GetPlayerByIdAsync(int id);
        Task CreatePlayerAsync(Player player);
        Task<bool> UpdatePlayerAsync(Player player);
        Task<bool> DeletePlayerAsync(int id);
        Task<bool> PlayerExistsAsync(int id);
    }
}