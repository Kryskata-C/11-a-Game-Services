using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models; 
using System.Security.Claims;


namespace WebApplication1.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; 

        public ReviewService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Review> GetReviewByIdAsync(int id)
        {
            return await _context.Reviews.FindAsync(id);
        }

        public async Task<Review> GetReviewWithPlayerAndTeamAsync(int id)
        {
            return await _context.Reviews
                                 .Include(r => r.Player) 
                                 .Include(r => r.Team)   
                                 .AsNoTracking() 
                                 .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddPlayerReviewAsync(AddReviewViewModel model, string userId, string reviewerName)
        {
            var review = new Review
            {
                PlayerId = model.PlayerId,
                ReviewerName = reviewerName, 
                CommentText = model.CommentText,
                StarRating = model.StarRating,
                ReviewDate = DateTime.UtcNow,
                UserId = userId
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateReviewAsync(EditReviewViewModel model, string currentUserId, bool isAdmin)
        {
            var reviewToUpdate = await _context.Reviews.FindAsync(model.Id);
            if (reviewToUpdate == null) return false;

            if (reviewToUpdate.UserId != currentUserId && !isAdmin) return false; 

            reviewToUpdate.CommentText = model.CommentText;
            reviewToUpdate.StarRating = model.StarRating;
            reviewToUpdate.ReviewDate = DateTime.UtcNow; 

            try
            {
                _context.Update(reviewToUpdate);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ReviewExistsAsync(model.Id)) return false; 
                else throw; 
            }
            catch (DbUpdateException)
            {
                return false; 
            }
        }

        public async Task<(bool Success, Review ReviewToDelete)> CanDeleteReviewAsync(int id, string currentUserId, bool isAdmin)
        {
            var review = await GetReviewByIdAsync(id); 
            if (review == null) return (false, null);
            if (review.UserId != currentUserId && !isAdmin) return (false, review); 
            return (true, review);
        }


        public async Task DeleteReviewAsync(int id)
        {
            var reviewToDelete = await _context.Reviews.FindAsync(id);
            if (reviewToDelete != null)
            {
                _context.Reviews.Remove(reviewToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ReviewExistsAsync(int id)
        {
            return await _context.Reviews.AnyAsync(e => e.Id == id);
        }

        public async Task<Player> GetPlayerForReviewAsync(int playerId)
        {
            return await _context.Players.AsNoTracking().FirstOrDefaultAsync(p => p.Id == playerId);
        }
    }
}