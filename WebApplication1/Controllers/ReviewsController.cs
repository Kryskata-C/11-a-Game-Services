using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication1.Models;    
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


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

        [Authorize] 
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews
                                       .Include(r => r.Player) 
                                       .Include(r => r.Team)   
                                       .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (review.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var viewModel = new EditReviewViewModel
            {
                Id = review.Id,
                PlayerId = review.PlayerId,
                PlayerName = review.Player?.GamerTag, 
                TeamId = review.TeamId,
                TeamName = review.Team?.Name, 
                ReviewerName = review.ReviewerName,
                CommentText = review.CommentText,
                StarRating = review.StarRating,
                OriginalUserId = review.UserId 
            };

            if (review.PlayerId.HasValue)
            {
                ViewData["ReviewSubjectName"] = review.Player?.GamerTag ?? "Player";
                ViewData["ReviewSubjectType"] = "Player";
            }
            else if (review.TeamId.HasValue)
            {
                ViewData["ReviewSubjectName"] = review.Team?.Name ?? "Team";
                ViewData["ReviewSubjectType"] = "Team";
            }
            else
            {
                ViewData["ReviewSubjectName"] = "Item"; 
                ViewData["ReviewSubjectType"] = "Item";
            }


            return View(viewModel);
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditReviewViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (model.PlayerId.HasValue)
            {
                ViewData["ReviewSubjectName"] = model.PlayerName ?? "Player"; 
                ViewData["ReviewSubjectType"] = "Player";
            }
            else if (model.TeamId.HasValue)
            {
                ViewData["ReviewSubjectName"] = model.TeamName ?? "Team"; 
                ViewData["ReviewSubjectType"] = "Team";
            }
            else
            {
                ViewData["ReviewSubjectName"] = "Item";
                ViewData["ReviewSubjectType"] = "Item";
            }

            if (ModelState.IsValid)
            {
                var reviewToUpdate = await _context.Reviews.FindAsync(model.Id);

                if (reviewToUpdate == null)
                {
                    return NotFound();
                }

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (reviewToUpdate.UserId != currentUserId && !User.IsInRole("Admin"))
                {
                    TempData["ErrorMessage"] = "You are not authorized to edit this review.";
                    if (reviewToUpdate.PlayerId.HasValue)
                        return RedirectToAction("Details", "Players", new { id = reviewToUpdate.PlayerId });
                    if (reviewToUpdate.TeamId.HasValue)
                        return RedirectToAction("Details", "Teams", new { id = reviewToUpdate.TeamId });
                    return RedirectToAction("Index", "Home"); 
                }

                reviewToUpdate.CommentText = model.CommentText;
                reviewToUpdate.StarRating = model.StarRating;
                reviewToUpdate.ReviewDate = DateTime.UtcNow; 

                try
                {
                    _context.Update(reviewToUpdate);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Review updated successfully!";

\                    if (reviewToUpdate.PlayerId.HasValue)
                    {
                        return RedirectToAction("Details", "Players", new { id = reviewToUpdate.PlayerId });
                    }
                    else if (reviewToUpdate.TeamId.HasValue)
                    {
                        return RedirectToAction("Details", "Teams", new { id = reviewToUpdate.TeamId });
                    }
\                    return RedirectToAction("Index", "Home");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Reviews.AnyAsync(e => e.Id == model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The review was modified by another user. Please try again.");
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

\            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddReviewViewModel model)
        {
            var player = await _context.Players
                                     .Include(p => p.PlayerReviews)
                                     .FirstOrDefaultAsync(p => p.Id == model.PlayerId);
            if (player == null)
            {
                ModelState.AddModelError("", "Player not found. Unable to add review.");
            }

            string currentUserId = null;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    currentUserId = currentUser.Id;
                    if (string.IsNullOrWhiteSpace(model.ReviewerName))
                    {
                        model.ReviewerName = currentUser.UserName;
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(model.ReviewerName) && ModelState.ContainsKey("ReviewerName"))
            {
            }


            if (ModelState.IsValid && player != null)
            {
                var review = new WebApplication1.Models.Review 
                {
                    PlayerId = model.PlayerId,
                    ReviewerName = model.ReviewerName, 
                    CommentText = model.CommentText,
                    StarRating = model.StarRating,
                    ReviewDate = DateTime.UtcNow,
                    UserId = currentUserId 
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