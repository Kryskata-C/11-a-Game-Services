using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using WebApplication1.Models; // If Player is in Models namespace
// Ensure your Player model is accessible, e.g. via a using directive if it's in a different namespace.
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
            // Removed .Include(p => p.ApplicationUser) as it's no longer part of Player
            return await _context.Players.Include(p => p.Team).ToListAsync(); // Include Team if you want team details
        }

        public async Task<Player> GetPlayerByIdAsync(int id)
        {
            // Removed .Include(p => p.ApplicationUser)
            return await _context.Players
                .Include(p => p.Team) // Include Team if you want team details
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreatePlayerAsync(Player player)
        {
            // Removed: player.RegistrationDate = System.DateTime.UtcNow;
            // Any other default values for new properties can be set here if needed
            // For example, if Rating should default to 0 or PricePerHour to a base value.
            // player.Rating = 0; // Example if new players should have 0 rating initially.
            _context.Add(player);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdatePlayerAsync(Player player)
        {
            // Ensure the player object being passed in has its Id set correctly.
            // The state of the entity will be marked as Modified.
            // Detach any existing tracked entity with the same Id if necessary,
            // or fetch the existing entity and update its properties.
            // A simple _context.Update(player) can lead to issues if not handled carefully with detached entities.

            var existingPlayer = await _context.Players.FindAsync(player.Id);
            if (existingPlayer == null)
            {
                return false; // Or throw not found exception
            }

            // Update properties from the passed player object to the existingPlayer
            _context.Entry(existingPlayer).CurrentValues.SetValues(player);
            // If you have navigation properties that might change (e.g., TeamId), handle them explicitly:
            // existingPlayer.TeamId = player.TeamId;


            // _context.Update(player); // This can sometimes be problematic if player is not tracked or if only partial data is sent.
            // Using FindAsync and then updating properties is safer.
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                // This catch block correctly checks if the player still exists.
                if (!await PlayerExistsAsync(player.Id))
                {
                    return false; // Player was deleted.
                }
                else
                {
                    throw; // Concurrency conflict (player modified by someone else).
                }
            }
            catch (DbUpdateException) // Catch other potential update issues
            {
                // Log error, handle specific DB constraints if necessary
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