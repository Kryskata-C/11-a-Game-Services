using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models; 

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")] 
    [ApiController] 
    public class ReviewsApiController : ControllerBase 
    {
        private readonly ApplicationDbContext _context;

        public ReviewsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetReviewsForPlayer/{playerId}")]
        public async Task<IActionResult> GetReviewsForPlayer(int playerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var playerExists = await _context.Players.AnyAsync(p => p.Id == playerId);
            if (!playerExists)
            {
                return NotFound(new { message = "Player not found." });
            }

            var query = _context.Reviews
                                .Where(r => r.PlayerId == playerId)
                                .OrderByDescending(r => r.ReviewDate);

            var totalReviews = await query.CountAsync();
            var reviewsForPage = await query.Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalReviews / (double)pageSize);
            if (totalPages == 0) totalPages = 1; 

            var reviewsDto = reviewsForPage.Select(r => new
            {
                r.Id,
                r.ReviewerName,
                r.CommentText,
                ReviewDate = r.ReviewDate.ToString("yyyy-MM-dd HH:mm"), 
                r.StarRating
            }).ToList();

            return Ok(new 
            {
                PlayerId = playerId,
                Reviews = reviewsDto,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalReviews = totalReviews
            });
        }
    }
}