// File: WebApplication1/Controllers/ReviewsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication1.Data;     // For ApplicationDbContext
using WebApplication1.Models;    // CRUCIAL: For Review, AddReviewViewModel, ApplicationUser, Player
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System;                   // For DateTime

namespace WebApplication1.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                return NotFound("Player not found.");
            }

            var viewModel = new AddReviewViewModel { PlayerId = playerId };

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    viewModel.ReviewerName = currentUser.UserName;
                }
            }

            ViewData["PlayerName"] = player.GamerTag;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddReviewViewModel model)
        {
            var player = await _context.Players
                                     .Include(p => p.PlayerReviews) // Make sure Player.cs uses PlayerReviews
                                     .FirstOrDefaultAsync(p => p.Id == model.PlayerId);
            if (player == null)
            {
                ModelState.AddModelError("", "Player not found. Unable to add review.");
            }

            if (ModelState.IsValid && player != null)
            {
                var review = new WebApplication1.Models.Review
                {
                    PlayerId = model.PlayerId,         
                    ReviewerName = model.ReviewerName,
                    CommentText = model.CommentText,   
                    StarRating = model.StarRating,
                    ReviewDate = DateTime.UtcNow      
                };

                _context.Reviews.Add(review); 
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Review submitted successfully!";
                return RedirectToAction("Details", "Players", new { id = model.PlayerId });
            }

            if (player != null)
            {
                ViewData["PlayerName"] = player.GamerTag;
            }
            else
            {
                ViewData["PlayerName"] = "Selected Player";
            }
            return View(model);
        }
    }
}