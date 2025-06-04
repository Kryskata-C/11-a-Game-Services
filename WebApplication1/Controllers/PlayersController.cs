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

    public async Task<IActionResult> PlayerHire(
        string currentSearchTerm,
        string searchTerm,
        string sortOrder,
        int? pageNumber)
    {
        ViewBag.IsAdmin = User.IsInRole("Admin"); 

        if (searchTerm != null)
        {
            pageNumber = 1; 
        }
        else
        {
            searchTerm = currentSearchTerm; 
        }

        if (string.IsNullOrEmpty(sortOrder))
        {
            sortOrder = "gamertag_asc"; 
        }

        int pageSize = 6;
        int currentPageNumber = pageNumber ?? 1;

        var serviceResult = await _playerService.GetAllPlayersAsync(searchTerm, sortOrder, currentPageNumber, pageSize);

        var totalPages = (int)Math.Ceiling(serviceResult.TotalCount / (double)pageSize);
        if (totalPages == 0) totalPages = 1; 

        var viewModel = new PlayerHireViewModel
        {
            Players = serviceResult.Players,
            PageNumber = currentPageNumber,
            TotalPages = totalPages,
            CurrentSortOrder = sortOrder,
            CurrentSearchTerm = searchTerm,
            GamerTagSortParam = sortOrder == "gamertag_asc" ? "gamertag_desc" : "gamertag_asc",
            PriceSortParam = sortOrder == "price_asc" ? "price_desc" : "price_asc",
            RatingSortParam = sortOrder == "rating_asc" ? "rating_desc" : "rating_asc"
        };

        return View("PlayerHire", viewModel); 
    }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(PlayerHire));
    }

    public IActionResult Create()
    {
        ViewBag.IsAdmin = IsAdminUser();
        return View(new PlayerCreateViewModel());
    }

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,GamerTag,Description,PricePerHour,Rating,ImageUrl,TeamId")] Player player)
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

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _playerService.DeletePlayerAsync(id);
        TempData["SuccessMessage"] = "Player deleted successfully!";
        return RedirectToAction(nameof(PlayerHire));
    }

    public async Task<IActionResult> Details(int? id) 
    {
        ViewBag.IsAdmin = IsAdminUser(); 
        if (id == null)
        {
            return NotFound();
        }

        var player = await _context.Players
            .Include(p => p.Team)
            .FirstOrDefaultAsync(p => p.Id == id.Value);

        if (player == null)
        {
            return NotFound();
        }

        var viewModel = new PlayerDetailViewModel
        {
            Player = player
        };

        return View(viewModel);
    }
}