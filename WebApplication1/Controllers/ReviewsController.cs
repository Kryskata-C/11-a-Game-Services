using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebApplication1.Services;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(IReviewService reviewService, UserManager<ApplicationUser> userManager)
        {
            _reviewService = reviewService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int playerId)
        {
            var player = await _reviewService.GetPlayerForReviewAsync(playerId);
            if (player == null) return NotFound("Player not found.");

            var currentUser = await _userManager.GetUserAsync(User);
            var viewModel = new AddReviewViewModel
            {
                PlayerId = playerId,
                ReviewerName = currentUser?.UserName
            };
            ViewData["PlayerName"] = player.GamerTag;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddReviewViewModel model)
        {
            Player player = null;
            if (model.PlayerId.HasValue)
            {
                player = await _reviewService.GetPlayerForReviewAsync(model.PlayerId.Value);
                if (player == null)
                {
                    ModelState.AddModelError("PlayerId", "Associated player not found.");
                }
            }

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                string reviewerName = string.IsNullOrWhiteSpace(model.ReviewerName) ? (currentUser?.UserName ?? "Anonymous") : model.ReviewerName;

                try
                {
                    await _reviewService.AddPlayerReviewAsync(model, currentUser?.Id, reviewerName);
                    TempData["SuccessMessage"] = "Review submitted successfully!";
                    if (model.PlayerId.HasValue)
                        return RedirectToAction("Details", "Players", new { id = model.PlayerId.Value });

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Could not save review: {ex.Message}");
                }
            }

            if (model.PlayerId.HasValue && player != null)
            {
                ViewData["PlayerName"] = player.GamerTag;
            }
            else if (model.PlayerId.HasValue)
            {
                ViewData["PlayerName"] = "Selected Player";
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _reviewService.GetReviewWithPlayerAndTeamAsync(id);
            if (review == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (review.UserId != currentUserId && !User.IsInRole("Admin")) return Forbid();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditReviewViewModel model)
        {
            if (id != model.Id) return NotFound();

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
                var currentUserId = _userManager.GetUserId(User);
                var isAdmin = User.IsInRole("Admin");
                try
                {
                    var success = await _reviewService.UpdateReviewAsync(model, currentUserId, isAdmin);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Review updated successfully!";
                        if (model.PlayerId.HasValue)
                            return RedirectToAction("Details", "Players", new { id = model.PlayerId.Value });
                        if (model.TeamId.HasValue)
                            return RedirectToAction("Details", "Teams", new { id = model.TeamId.Value });
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Could not update review. It may have been modified or you lack permission.");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError(string.Empty, "The review was modified by another user. Please try again.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error updating review: {ex.Message}");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, bool? saveChangesError = false)
        {
            var review = await _reviewService.GetReviewWithPlayerAndTeamAsync(id);
            if (review == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (review.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "You are not authorized to delete this review.";
                if (review.PlayerId.HasValue)
                    return RedirectToAction("Details", "Players", new { id = review.PlayerId.Value });
                if (review.TeamId.HasValue)
                    return RedirectToAction("Details", "Teams", new { id = review.TeamId.Value });
                return RedirectToAction("Index", "Home");
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed. Try again.";
            }

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
            return View(review);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int? PlayerId, int? TeamId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            var (canDelete, reviewToDelete) = await _reviewService.CanDeleteReviewAsync(id, currentUserId, isAdmin);

            if (reviewToDelete == null)
            {
                TempData["ErrorMessage"] = "Review not found or already deleted.";
            }
            else if (!canDelete)
            {
                TempData["ErrorMessage"] = "You are not authorized to delete this review.";
            }
            else
            {
                try
                {
                    await _reviewService.DeleteReviewAsync(id);
                    TempData["SuccessMessage"] = "Review deleted successfully.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error deleting review: {ex.Message}";
                    return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
                }
            }

            if (PlayerId.HasValue) return RedirectToAction("Details", "Players", new { id = PlayerId.Value });
            if (TeamId.HasValue) return RedirectToAction("Details", "Teams", new { id = TeamId.Value });
            return RedirectToAction("Index", "Home");
        }
    }
}