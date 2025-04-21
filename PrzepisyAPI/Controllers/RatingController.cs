using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrzepisyAPI.Db;
using PrzepisyAPI.Dtos;
using PrzepisyAPI.Models;
using System.Security.Claims;

namespace PrzepisyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly RecipeDbContext _context;
        public RatingController(RecipeDbContext context) => _context = context;

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Rating>> Get()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return await _context.Ratings
                .Where(r => r.UserId == userId)
                .Include(r => r.Recipe)
                .ToListAsync();
        }

        [Authorize]
        [HttpPost("rating")]
        public async Task<IActionResult> Rate([FromBody] RatingDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var existing = await _context.Ratings
                .FirstOrDefaultAsync(r => r.RecipeId == dto.RecipeId && r.UserId == userId);

            if (existing != null)
                existing.Score = dto.Score;
            else
                _context.Ratings.Add(new Rating
                {
                    RecipeId = dto.RecipeId,
                    UserId = userId,
                    Score = dto.Score,
                    CreatedAt = DateTime.Now
                });

            await _context.SaveChangesAsync();
            return Ok();
        }

    }

}
