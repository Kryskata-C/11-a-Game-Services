using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Data.Entities;
using WebApplication1.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

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
                Rating = 0, 
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
                    model.AvailablePlayers = await GetAvailablePlayersForCreateAsync(model.SelectedPlayerIds);
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
        model.AvailablePlayers = await GetAvailablePlayersForCreateAsync(model.SelectedPlayerIds);
        return View(model);
    }

    private async Task<List<SelectListItem>> GetAvailablePlayersForCreateAsync(List<int> alreadySelectedPlayerIds)
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
    
    public async Task<IActionResult> Index(
    string currentSearchTerm,
    string searchTerm,
    string sortOrder,
    int? pageNumber)
    {
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
            sortOrder = "name_asc";
        }

        var query = _context.Teams
                            .Include(t => t.Players)
                            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(t => t.Name.Contains(searchTerm) ||
                                     (t.Description != null && t.Description.Contains(searchTerm)));
        }

        switch (sortOrder)
        {
            case "name_desc":
                query = query.OrderByDescending(t => t.Name);
                break;
            case "price_asc":
                query = query.OrderBy(t => t.PricePerHour);
                break;
            case "price_desc":
                query = query.OrderByDescending(t => t.PricePerHour);
                break;
            case "rating_asc":
                query = query.OrderBy(t => t.Rating);
                break;
            case "rating_desc":
                query = query.OrderByDescending(t => t.Rating);
                break;
            case "date_asc":
                query = query.OrderBy(t => t.DateCreated);
                break;
            case "date_desc":
                query = query.OrderByDescending(t => t.DateCreated);
                break;
            case "name_asc": 
            default:
                query = query.OrderBy(t => t.Name);
                break;
        }

        int pageSize = 6; 
        int currentPageNumber = pageNumber ?? 1;
        var totalCount = await query.CountAsync();
        var teamsForPage = await query
                                  .Skip((currentPageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        if (totalPages == 0) totalPages = 1;

        var viewModel = new TeamIndexViewModel
        {
            Teams = teamsForPage,
            PageNumber = currentPageNumber,
            TotalPages = totalPages,
            CurrentSortOrder = sortOrder,
            CurrentSearchTerm = searchTerm,
            NameSortParam = sortOrder == "name_asc" ? "name_desc" : "name_asc",
            PriceSortParam = sortOrder == "price_asc" ? "price_desc" : "price_asc",
            RatingSortParam = sortOrder == "rating_asc" ? "rating_desc" : "rating_asc",
            DateCreatedSortParam = sortOrder == "date_asc" ? "date_desc" : "date_asc"
        };

        return View(viewModel);
    }

    [HttpGet]
    [Authorize] 
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var team = await _context.Teams
                                 .Include(t => t.Players) 
                                 .FirstOrDefaultAsync(t => t.Id == id);

        if (team == null)
        {
            return NotFound();
        }
        
        var viewModel = new TeamEditViewModel
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
            PricePerHour = team.PricePerHour,
            ExistingImageUrl = team.ImageUrl,
            SelectedPlayerIds = team.Players.Select(p => p.Id).ToList(),
            AvailablePlayers = await _context.Players
                .Where(p => p.TeamId == null || p.TeamId == team.Id) 
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.GamerTag,
                    Selected = team.Players.Any(tp => tp.Id == p.Id) 
                })
                .OrderBy(p => p.Text)
                .ToListAsync()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, TeamEditViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var teamToUpdate = await _context.Teams
                                             .Include(t => t.Players)
                                             .FirstOrDefaultAsync(t => t.Id == model.Id);

            if (teamToUpdate == null)
            {
                return NotFound($"Team with ID {model.Id} not found.");
            }

            teamToUpdate.Name = model.Name;
            teamToUpdate.Description = model.Description;
            teamToUpdate.PricePerHour = model.PricePerHour;

            if (model.NewTeamImageFile != null && model.NewTeamImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(teamToUpdate.ImageUrl) && teamToUpdate.ImageUrl != "/images/default-team.png")
                {
                    string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, teamToUpdate.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        catch (IOException)
                        {
                            // Log error or handle
                        }
                    }
                }
                
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "teamlogos");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.NewTeamImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.NewTeamImageFile.CopyToAsync(fileStream);
                    }
                    teamToUpdate.ImageUrl = "/images/teamlogos/" + uniqueFileName;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(model.NewTeamImageFile), $"File upload failed: {ex.Message}");
                    model.AvailablePlayers = await GetAvailablePlayersForEditAsync(teamToUpdate.Id, model.SelectedPlayerIds ?? new List<int>());
                    model.ExistingImageUrl = teamToUpdate.ImageUrl;
                    return View(model);
                }
            }
            
            var selectedPlayerIds = model.SelectedPlayerIds ?? new List<int>();
            var currentAssignedPlayerIds = teamToUpdate.Players.Select(p => p.Id).ToList();

            var playersToRemove = teamToUpdate.Players
                                    .Where(p => !selectedPlayerIds.Contains(p.Id))
                                    .ToList(); 
            foreach (var player in playersToRemove)
            {
                player.TeamId = null; 
            }
            
            var playerIdsToAdd = selectedPlayerIds
                                    .Except(currentAssignedPlayerIds)
                                    .ToList();

            foreach (var playerIdToAdd in playerIdsToAdd)
            {
                var player = await _context.Players.FindAsync(playerIdToAdd);
                if (player != null)
                {
                    if (player.TeamId == null || player.TeamId == teamToUpdate.Id) 
                    {
                        player.TeamId = teamToUpdate.Id;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Player {player.GamerTag} is already assigned to another team.");
                        model.AvailablePlayers = await GetAvailablePlayersForEditAsync(teamToUpdate.Id, model.SelectedPlayerIds ?? new List<int>());
                        model.ExistingImageUrl = teamToUpdate.ImageUrl;
                        return View(model);
                    }
                }
            }

            try
            {
                _context.Update(teamToUpdate);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Team '{teamToUpdate.Name}' updated successfully!";
                return RedirectToAction("Details", new { id = teamToUpdate.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TeamExists(teamToUpdate.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The record was modified by another user. Your edit operation was canceled. Please try again.");
                }
            }
            catch (DbUpdateException ex)
            {
                 ModelState.AddModelError("", "Unable to save changes. Error: " + ex.Message);
            }
        }
        
        model.AvailablePlayers = await GetAvailablePlayersForEditAsync(model.Id, model.SelectedPlayerIds ?? new List<int>());
        model.ExistingImageUrl ??= (await _context.Teams.AsNoTracking().FirstOrDefaultAsync(t => t.Id == model.Id))?.ImageUrl;


        return View(model);
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
    {
        if (id == null)
        {
            return NotFound();
        }

        var team = await _context.Teams
            .AsNoTracking()
            .Include(t => t.Players)
            .Include(t => t.Reviews)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (team == null)
        {
            return NotFound();
        }

        if (saveChangesError.GetValueOrDefault())
        {
            ViewData["ErrorMessage"] =
                "Delete failed. Try again, and if the problem persists " +
                "see your system administrator.";
        }

        return View(team);
    }
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var teamToDelete = await _context.Teams
                                         .Include(t => t.Players) 
                                         .Include(t => t.Reviews) 
                                         .FirstOrDefaultAsync(t => t.Id == id);

        if (teamToDelete == null)
        {
            TempData["ErrorMessage"] = "Team not found or already deleted.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            if (!string.IsNullOrEmpty(teamToDelete.ImageUrl) && teamToDelete.ImageUrl != "/images/default-team.png")
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, teamToDelete.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Error deleting image {imagePath}: {ex.Message}"); // Replace with proper logging
                    }
                }
            }

            _context.Teams.Remove(teamToDelete);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Team '{teamToDelete.Name}' and its associated reviews have been successfully deleted. Players formerly on this team are now unassigned.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException /* ex */)
        {
            TempData["ErrorMessage"] = "Unable to delete team. Try again, and if the problem persists, see your system administrator.";
            return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
        }
    }
    private async Task<bool> TeamExists(int id)
    {
        return await _context.Teams.AnyAsync(e => e.Id == id);
    }

    private async Task<List<SelectListItem>> GetAvailablePlayersForEditAsync(int teamId, List<int> currentSelectedPlayerIdsOnForm)
    {
        var players = await _context.Players
            .Where(p => p.TeamId == null || p.TeamId == teamId)
            .OrderBy(p => p.GamerTag)
            .ToListAsync();

        return players.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.GamerTag,
                Selected = currentSelectedPlayerIdsOnForm.Contains(p.Id) 
            })
            .ToList();
    }
}