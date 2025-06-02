using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectList
using Microsoft.EntityFrameworkCore; // Required for _context and .OrderBy
using System.Linq; // Required for .OrderBy
using System.Threading.Tasks;
using WebApplication1.Services; // Your IPlayerService
// Assuming your Player model is in the default Models folder or WebApplication1 namespace
// If Player is in WebApplication1.Models, ensure: using WebApplication1.Models;
// Assuming ApplicationDbContext is in the default Data folder or WebApplication1 namespace
// If ApplicationDbContext is in WebApplication1.Data, ensure: using WebApplication1.Data;
// Assuming ApplicationUser is in WebApplication1.Data.Entities or WebApplication1
// If ApplicationUser is in WebApplication1.Data.Entities, ensure: using WebApplication1.Data.Entities;
using Microsoft.AspNetCore.Identity; // For User.IsInRole if not using UserManager

public class PlayersController : Controller
{
    private readonly IPlayerService _playerService;
    private readonly ApplicationDbContext _context; // Added for accessing Teams for dropdown
    // private readonly UserManager<ApplicationUser> _userManager; // Uncomment if you need UserManager for roles

    // Updated constructor to inject ApplicationDbContext
    public PlayersController(IPlayerService playerService, ApplicationDbContext context /*, UserManager<ApplicationUser> userManager */)
    {
        _playerService = playerService;
        _context = context; // Initialize DbContext
        // _userManager = userManager; // Uncomment if using UserManager
    }

    // PlayerHire action to serve PlayerHire.cshtml
    public async Task<IActionResult> PlayerHire()
    {
        ViewBag.IsAdmin = User.IsInRole("Admin");
        var players = await _playerService.GetAllPlayersAsync();
        if (players == null)
        {
            players = new List<Player>();
        }
        return View("PlayerHire", players);
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.IsAdmin = User.IsInRole("Admin");
        var players = await _playerService.GetAllPlayersAsync();
        return View(players); // This should point to Views/Players/Index.cshtml
    }

    public async Task<IActionResult> Details(int? id)
    {
        ViewBag.IsAdmin = User.IsInRole("Admin");
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

    public IActionResult Create()
    {
        ViewBag.IsAdmin = User.IsInRole("Admin");
        // Populate ViewBag for Team dropdown
        ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(t => t.Name), "Id", "Name");
        return View(new Player());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // Ensure Bind includes ImageUrl if it's part of your Player model and form
    public async Task<IActionResult> Create([Bind("GamerTag,Description,PricePerHour,Rating,Reviews,ImageUrl,TeamId")] Player player)
    {
        ViewBag.IsAdmin = User.IsInRole("Admin"); // In case of returning to View with errors
        if (ModelState.IsValid)
        {
            await _playerService.CreatePlayerAsync(player);
            return RedirectToAction(nameof(PlayerHire)); // Or nameof(Index) if that's your main list
        }
        // If ModelState is invalid, repopulate ViewBag for Team dropdown before returning view
        ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(t => t.Name), "Id", "Name", player.TeamId);
        return View(player);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        ViewBag.IsAdmin = User.IsInRole("Admin");
        if (id == null)
        {
            return NotFound();
        }
        var player = await _playerService.GetPlayerByIdAsync(id.Value);
        if (player == null)
        {
            return NotFound();
        }
        // Populate ViewBag for Team dropdown, pre-selecting current team
        ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(t => t.Name), "Id", "Name", player.TeamId);
        return View(player);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // Ensure Bind includes ImageUrl
    public async Task<IActionResult> Edit(int id, [Bind("Id,GamerTag,Description,PricePerHour,Rating,Reviews,ImageUrl,TeamId")] Player player)
    {
        ViewBag.IsAdmin = User.IsInRole("Admin"); // In case of returning to View with errors
        if (id != player.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var updated = await _playerService.UpdatePlayerAsync(player);
            if (!updated)
            {
                if (!await _playerService.PlayerExistsAsync(player.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred updating the player. It might have been modified or deleted. Please try again.");
                    // Repopulate ViewBag for Team dropdown if returning to view with error
                    ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(t => t.Name), "Id", "Name", player.TeamId);
                    return View(player);
                }
            }
            return RedirectToAction(nameof(PlayerHire)); // Or nameof(Index)
        }
        // If ModelState is invalid, repopulate ViewBag for Team dropdown
        ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(t => t.Name), "Id", "Name", player.TeamId);
        return View(player);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        ViewBag.IsAdmin = User.IsInRole("Admin");
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
        return RedirectToAction(nameof(PlayerHire)); // Or nameof(Index)
    }
}