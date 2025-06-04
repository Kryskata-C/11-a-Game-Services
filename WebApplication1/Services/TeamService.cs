using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data.Entities;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeamService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<(IEnumerable<Team> Teams, int TotalCount)> GetTeamsAsync(string searchTerm, string sortOrder, int pageNumber, int pageSize)
        {
            var query = _context.Teams
                                .Include(t => t.Players) 
                                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => t.Name.Contains(searchTerm) ||
                                         (t.Description != null && t.Description.Contains(searchTerm)));
            }

            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(t => t.Name),
                "price_asc" => query.OrderBy(t => t.PricePerHour),
                "price_desc" => query.OrderByDescending(t => t.PricePerHour),
                "rating_asc" => query.OrderBy(t => t.Rating),
                "rating_desc" => query.OrderByDescending(t => t.Rating),
                "date_asc" => query.OrderBy(t => t.DateCreated),
                "date_desc" => query.OrderByDescending(t => t.DateCreated),
                _ => query.OrderBy(t => t.Name), 
            };

            var totalCount = await query.CountAsync();
            var teamsForPage = await query
                                      .Skip((pageNumber - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();
            return (teamsForPage, totalCount);
        }

        public async Task<Team> GetTeamByIdAsync(int id)
        {
            return await _context.Teams.FindAsync(id);
        }

        public async Task<Team> GetTeamWithDetailsAsync(int id)
        {
            return await _context.Teams
               .Include(t => t.Players)
               .Include(t => t.Reviews)
                   .ThenInclude(r => r.User) 
               .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateTeamAsync(Team team, List<int> selectedPlayerIds, string currentUserId, IEnumerable<AddReviewViewModel> initialTeamReviews, string webRootPath, string teamImageFileName)
        {
            // Image URL setting ( uniqueFileName from controller is the final name)
            team.ImageUrl = !string.IsNullOrEmpty(teamImageFileName) ? $"/images/teamlogos/{teamImageFileName}" : "/images/default-team.png";

            _context.Teams.Add(team);
            await _context.SaveChangesAsync(); 

            if (selectedPlayerIds != null && selectedPlayerIds.Any())
            {
                var playersToAssign = await _context.Players
                    .Where(p => selectedPlayerIds.Contains(p.Id) && p.TeamId == null) 
                    .ToListAsync();
                foreach (var player in playersToAssign)
                {
                    player.TeamId = team.Id;
                }
            }

            if (initialTeamReviews != null && initialTeamReviews.Any())
            {
                foreach (var reviewModel in initialTeamReviews.Where(r => !string.IsNullOrWhiteSpace(r.CommentText)))
                {
                    var review = new Review
                    {
                        TeamId = team.Id,
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
        }

        public async Task<bool> UpdateTeamAsync(TeamEditViewModel model, string webRootPath)
        {
            var teamToUpdate = await _context.Teams
                                             .Include(t => t.Players) 
                                             .FirstOrDefaultAsync(t => t.Id == model.Id);

            if (teamToUpdate == null) return false;

            teamToUpdate.Name = model.Name;
            teamToUpdate.Description = model.Description;
            teamToUpdate.PricePerHour = model.PricePerHour;

            if (model.NewTeamImageFile != null && model.NewTeamImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(teamToUpdate.ImageUrl) && teamToUpdate.ImageUrl != "/images/default-team.png")
                {
                    string oldImagePath = Path.Combine(webRootPath, teamToUpdate.ImageUrl.TrimStart('/'));
                    if (File.Exists(oldImagePath))
                    {
                        try { File.Delete(oldImagePath); } catch (IOException) { }
                    }
                }
                string uploadsFolder = Path.Combine(webRootPath, "images", "teamlogos");
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
                catch (Exception) { return false; }
            }


            var selectedPlayerIds = model.SelectedPlayerIds ?? new List<int>();

            var playersToRemove = teamToUpdate.Players.Where(p => !selectedPlayerIds.Contains(p.Id)).ToList();
            foreach (var player in playersToRemove)
            {
                player.TeamId = null;
            }

            var currentAssignedPlayerIds = teamToUpdate.Players.Select(p => p.Id).ToList();
            var playerIdsToAdd = selectedPlayerIds.Except(currentAssignedPlayerIds).ToList();
            foreach (var playerIdToAdd in playerIdsToAdd)
            {
                var player = await _context.Players.FindAsync(playerIdToAdd);
                if (player != null)
                {
                    if (player.TeamId != null && player.TeamId != teamToUpdate.Id)
                    {

                        return false;
                    }
                    player.TeamId = teamToUpdate.Id;
                }
            }

            try
            {
                _context.Update(teamToUpdate);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TeamExistsAsync(teamToUpdate.Id)) return false; 
                else throw; 
            }
            catch (DbUpdateException)
            {
                return false; 
            }
        }

        public async Task DeleteTeamAsync(int id, string webRootPath)
        {
            var teamToDelete = await _context.Teams
                                     .Include(t => t.Reviews) 
                                     .FirstOrDefaultAsync(t => t.Id == id);

            if (teamToDelete == null) return;

            if (!string.IsNullOrEmpty(teamToDelete.ImageUrl) && teamToDelete.ImageUrl != "/images/default-team.png")
            {
                string imagePath = Path.Combine(webRootPath, teamToDelete.ImageUrl.TrimStart('/'));
                if (File.Exists(imagePath))
                {
                    try { File.Delete(imagePath); } catch (IOException) { }
                }
            }


            _context.Teams.Remove(teamToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TeamExistsAsync(int id)
        {
            return await _context.Teams.AnyAsync(e => e.Id == id);
        }

        public async Task<List<SelectListItem>> GetAvailablePlayersForCreateAsync(List<int> alreadySelectedPlayerIds)
        {
            return await _context.Players
               .Where(p => p.TeamId == null || (alreadySelectedPlayerIds != null && alreadySelectedPlayerIds.Contains(p.Id))) 
               .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.GamerTag })
               .OrderBy(p => p.Text)
               .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetAvailablePlayersForEditAsync(int teamId, List<int> currentSelectedPlayerIdsOnForm)
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
}