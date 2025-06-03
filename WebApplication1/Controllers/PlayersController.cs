// File: WebApplication1/Controllers/PlayersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Services;     // Your service namespace
using WebApplication1.Models;     // Your models namespace (for Player, PlayerCreateViewModel)
using Microsoft.AspNetCore.Hosting; // For IWebHostEnvironment
using System.IO;                    // For Path, FileStream, Directory
using Microsoft.AspNetCore.Http;    // For IFormFile (though usually referenced via model)
using System;                       // For Guid

public class PlayersController : Controller
{
    private readonly IPlayerService _playerService;
    private readonly ApplicationDbContext _context; // For Edit actions if they still use Team
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PlayersController(IPlayerService playerService, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _playerService = playerService;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    private bool IsAdminUser()
    {
        return HttpContext.Session.GetString("IsAdmin") == "true";
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

    public async Task<IActionResult> Details(int? id)
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

    // GET: Players/Create
    public IActionResult Create()
    {
        ViewBag.IsAdmin = IsAdminUser();
        return View(new PlayerCreateViewModel()); // Use the ViewModel
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
                    // Log the error (using a proper logging framework is recommended)
                    ModelState.AddModelError(string.Empty, $"Error uploading image: {ex.Message}");
                    return View(viewModel); // Return to view with error
                }
            }

            Player newPlayer = new Player
            {
                GamerTag = viewModel.GamerTag,
                Description = viewModel.Description,
                PricePerHour = viewModel.PricePerHour,
                Rating = viewModel.Rating,
                Reviews = viewModel.Reviews,
                ImageUrl = uniqueFileName != null ? $"/images/players/{uniqueFileName}" : null
                // TeamId will be null by default as it's not in the ViewModel or Player entity mapping here
            };

            try
            {
                await _playerService.CreatePlayerAsync(newPlayer);
                TempData["SuccessMessage"] = "Player created successfully!";
                return RedirectToAction(nameof(PlayerHire));
            }
            catch (Exception ex)
            {
                // Log the error from service layer
                ModelState.AddModelError(string.Empty, $"Error saving player: {ex.Message}");
                // If image was saved but player saving failed, you might want to delete the orphaned image.
                // This logic is omitted for brevity but is important in production.
            }
        }

        // If ModelState is invalid or an error occurred during processing, return to the Create view
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
        // If you want to edit with file upload, you'll need an EditViewModel similar to PlayerCreateViewModel
        // For now, assuming Player model is used directly for edit view and ImageUrl is a text path.
        // TeamId is still handled here for existing players.
        ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(t => t.Name), "Id", "Name", player.TeamId);
        return View(player);
    }

    // POST: Players/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,GamerTag,Description,PricePerHour,Rating,Reviews,ImageUrl,TeamId")] Player player)
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
                // Note: File upload logic is NOT included in this Edit POST action.
                // If you need to update the image, you'll need to add IFormFile to the parameters
                // (preferably via a ViewModel) and handle it similarly to the Create action.
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
}