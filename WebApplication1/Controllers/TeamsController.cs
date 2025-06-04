using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Data.Entities;
using WebApplication1.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System;

[Authorize]
public class TeamsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public TeamsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new TeamCreateViewModel
        {
            AvailablePlayers = await _context.Players
                .Where(p => p.TeamId == null)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.GamerTag })
                .OrderBy(p => p.Text)
                .ToListAsync()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeamCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            var team = new Team
            {
                Name = model.Name,
                Description = model.Description,
                PricePerHour = model.PricePerHour,
                Rating = 0, // Team rating will be 0 initially or calculated based on reviews later
                DateCreated = DateTime.UtcNow
            };

            if (model.TeamImageFile != null && model.TeamImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "teamlogos");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.TeamImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.TeamImageFile.CopyToAsync(fileStream);
                    }
                    team.ImageUrl = "/images/teamlogos/" + uniqueFileName;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(model.TeamImageFile), $"File upload failed: {ex.Message}");
                    model.AvailablePlayers = await GetAvailablePlayersAsync(model.SelectedPlayerIds);
                    return View(model);
                }
            }
            else
            {
                team.ImageUrl = "/images/default-team.png";
            }

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            if (model.SelectedPlayerIds != null && model.SelectedPlayerIds.Any())
            {
                var playersToAssign = await _context.Players
                                                .Where(p => model.SelectedPlayerIds.Contains(p.Id) && p.TeamId == null)
                                                .ToListAsync();
                foreach (var player in playersToAssign)
                {
                    player.TeamId = team.Id;
                }
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (model.InitialTeamReviews != null && model.InitialTeamReviews.Any())
            {
                foreach (var reviewModel in model.InitialTeamReviews.Where(r => !string.IsNullOrWhiteSpace(r.CommentText)))
                {
                    var review = new Review
                    {
                        TeamId = team.Id,
                        PlayerId = null,
                        ReviewerName = reviewModel.ReviewerName,
                        CommentText = reviewModel.CommentText,
                        StarRating = reviewModel.StarRating,
                        ReviewDate = DateTime.UtcNow,
                        UserId = currentUserId
                    };
                    _context.Reviews.Add(review);
                }
            }
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Team '{team.Name}' created successfully!";
            return RedirectToAction("Details", "Teams", new { id = team.Id });
        }
        model.AvailablePlayers = await GetAvailablePlayersAsync(model.SelectedPlayerIds);
        return View(model);
    }

    private async Task<List<SelectListItem>> GetAvailablePlayersAsync(List<int> alreadySelectedPlayerIds)
    {
        return await _context.Players
            .Where(p => p.TeamId == null || (alreadySelectedPlayerIds != null && alreadySelectedPlayerIds.Contains(p.Id)))
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.GamerTag })
            .OrderBy(p => p.Text)
            .ToListAsync();
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var team = await _context.Teams
            .Include(t => t.Players)
            .Include(t => t.Reviews)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (team == null)
        {
            return NotFound();
        }
        return View(team);
    }

    public async Task<IActionResult> Index()
    {
        var teams = await _context.Teams
                                .Include(t => t.Players)
                                .OrderBy(t => t.Name)
                                .ToListAsync();
        return View(teams);
    }
}