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

        public async Task<IEnumerable<Player>> GetAllPlayersAsync()
        {
            return await _context.Players.Include(p => p.ApplicationUser).ToListAsync();
        }

        public async Task<Player> GetPlayerByIdAsync(int id)
        {
            return await _context.Players
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreatePlayerAsync(Player player)
        {
            player.RegistrationDate = System.DateTime.UtcNow;
            _context.Add(player);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdatePlayerAsync(Player player)
        {
            _context.Update(player);
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