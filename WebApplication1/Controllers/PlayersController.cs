using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Still needed for Edit action (TeamId)
using Microsoft.EntityFrameworkCore;       // For Include, FirstOrDefaultAsync
using System.Linq;                         // For OrderBy, Skip, Take
using System.Threading.Tasks;
using WebApplication1.Services;
using WebApplication1.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;                       // For Guid
using System.Collections.Generic;   // For List

public class PlayersController : Controller
{
    private readonly IPlayerService _playerService;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PlayersController(IPlayerService playerService, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _playerService = playerService;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    private bool IsAdminUser()
    {
        return User.IsInRole("Admin");
    }

    public async Task<IActionResult> PlayerHire()
    {
        ViewBag.IsAdmin = IsAdminUser();
        var players = await _playerService.GetAllPlayersAsync();
        if (players == null)
        {
            players = new List<Player>();
        }
        return View("PlayerHire", players);
    }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(PlayerHire));
    }

    // GET: Players/Create
    public IActionResult Create()
    {
        ViewBag.IsAdmin = IsAdminUser();
        return View(new PlayerCreateViewModel());
    }

    // POST: Players/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlayerCreateViewModel viewModel)
    {
        ViewBag.IsAdmin = IsAdminUser();

        if (ModelState.IsValid)
        {
            string uniqueFileName = null;
            if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "players");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewModel.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ImageFile.CopyToAsync(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error uploading image: {ex.Message}");
                    return View(viewModel);
                }
            }

            // In PlayersController Create POST action
            Player newPlayer = new Player
            {
                GamerTag = viewModel.GamerTag,
                Description = viewModel.Description,
                PricePerHour = viewModel.PricePerHour,
                Rating = viewModel.Rating,
                ImageUrl = uniqueFileName != null ? $"/images/players/{uniqueFileName}" : null
            };

            if (viewModel.InitialReviews != null)
            {
                foreach (var reviewVm in viewModel.InitialReviews)
                {
                    var review = new Review
                    {
                        ReviewerName = reviewVm.ReviewerName,
                        CommentText = reviewVm.CommentText,
                        StarRating = reviewVm.StarRating,
                        ReviewDate = DateTime.UtcNow,
                        // Player will be set by EF Core when newPlayer is saved due to relationship
                    };
                    newPlayer.PlayerReviews.Add(review);
                }
            }



            try
            {
                await _playerService.CreatePlayerAsync(newPlayer);
                TempData["SuccessMessage"] = "Player created successfully!";
                return RedirectToAction(nameof(PlayerHire));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error saving player: {ex.Message}");
            }
        }
        return View(viewModel);
    }

    // GET: Players/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        ViewBag.IsAdmin = IsAdminUser();
        if (id == null)
        {
            return NotFound();
        }
        var player = await _playerService.GetPlayerByIdAsync(id.Value);
        if (player == null)
        {
            return NotFound();
        }
        ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(t => t.Name), "Id", "Name", player.TeamId);
        return View(player);
    }

    // POST: Players/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,GamerTag,Description,PricePerHour,Rating,ImageUrl,TeamId")] Player player)
    // IMPORTANT: Removed "Reviews" from Bind attribute here too, assuming it was the old string property.
    // If "Reviews" in the Bind was meant for the ICollection<Review> PlayerReviews,
    // binding collections of complex objects directly from forms is more complex and usually
    // handled differently (e.g., by managing individual review items, not binding the whole collection).
    // For now, this Edit POST does not handle updating the PlayerReviews collection.
    {
        ViewBag.IsAdmin = IsAdminUser();
        if (id != player.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var updated = await _playerService.UpdatePlayerAsync(player);
                if (!updated)
                {
                    if (!await _playerService.PlayerExistsAsync(player.Id)) { return NotFound(); }
                    ModelState.AddModelError(string.Empty, "Could not update player.");
                }
                else
                {
                    TempData["SuccessMessage"] = "Player updated successfully!";
                    return RedirectToAction(nameof(PlayerHire));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _playerService.PlayerExistsAsync(player.Id)) { return NotFound(); }
                ModelState.AddModelError(string.Empty, "The record was modified by another user. Please try again.");
            }
        }
        ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(t => t.Name), "Id", "Name", player.TeamId);
        return View(player);
    }

    // GET: Players/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        ViewBag.IsAdmin = IsAdminUser();
        if (id == null)
        {
            return NotFound();
        }
        var player = await _playerService.GetPlayerByIdAsync(id.Value);
        if (player == null)
        {
            return NotFound();
        }
        return View(player);
    }

    // POST: Players/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _playerService.DeletePlayerAsync(id);
        TempData["SuccessMessage"] = "Player deleted successfully!";
        return RedirectToAction(nameof(PlayerHire));
    }

    // Correct Details action with pagination for Reviews
    public async Task<IActionResult> Details(int? id, int page = 1)
    {
        ViewBag.IsAdmin = IsAdminUser();
        if (id == null)
        {
            return NotFound();
        }

        var player = await _context.Players
            .Include(p => p.Team)
            .Include(p => p.PlayerReviews) // Use the correct collection name: PlayerReviews
            .FirstOrDefaultAsync(p => p.Id == id.Value);

        if (player == null)
        {
            return NotFound();
        }

        int pageSize = 5;
        var allReviews = player.PlayerReviews != null ? player.PlayerReviews.OrderByDescending(r => r.ReviewDate).ToList() : new List<Review>();
        var reviewsForPage = allReviews.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var totalReviews = allReviews.Count;
        var totalPages = (int)Math.Ceiling(totalReviews / (double)pageSize);

        var viewModel = new PlayerDetailViewModel
        {
            Player = player,
            ReviewsOnCurrentPage = reviewsForPage,
            CurrentPage = page,
            TotalPages = totalPages > 0 ? totalPages : 1
        };

        return View(viewModel);
    }
}