using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrzepisyAPI.Db;
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

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Rating>> Post(Rating r)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var rating = new Rating
            {
                UserId = userId,
                RecipeId = r.RecipeId,
                Score = r.Score,
                CreatedAt = DateTime.Now
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = rating.Id }, rating);
        }
    }
}
