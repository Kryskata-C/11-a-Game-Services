using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly ApplicationDbContext _context;

        public PlayerService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<(IEnumerable<Player> Players, int TotalCount)> GetAllPlayersAsync(
            string searchTerm,
            string sortOrder,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Players.Include(p => p.Team).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.GamerTag.Contains(searchTerm) ||
                                         (p.Description != null && p.Description.Contains(searchTerm)));
            }

            
            switch (sortOrder)
            {
                case "gamertag_desc":
                    query = query.OrderByDescending(p => p.GamerTag);
                    break;
                case "price_asc":
                    query = query.OrderBy(p => p.PricePerHour);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.PricePerHour);
                    break;
                case "rating_asc":
                    query = query.OrderBy(p => p.Rating);
                    break;
                case "rating_desc":
                    query = query.OrderByDescending(p => p.Rating);
                    break;
                case "gamertag_asc": 
                default:
                    query = query.OrderBy(p => p.GamerTag);
                    break;
            }

            var totalCount = await query.CountAsync();

            var players = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            return (players, totalCount);
        }

        public async Task<Player> GetPlayerByIdAsync(int id)
        {
            return await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreatePlayerAsync(Player player)
        {
            _context.Add(player);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdatePlayerAsync(Player player)
        {
            var existingPlayer = await _context.Players.FindAsync(player.Id);
            if (existingPlayer == null)
            {
                return false;
            }
            _context.Entry(existingPlayer).CurrentValues.SetValues(player);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PlayerExistsAsync(player.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> DeletePlayerAsync(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return false;
            }
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PlayerExistsAsync(int id)
        {
            return await _context.Players.AnyAsync(e => e.Id == id);
        }
    }
}