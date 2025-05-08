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
    public class FavoriteController : ControllerBase
    {
        private readonly RecipeDbContext _context;
        public FavoriteController(RecipeDbContext context) => _context = context;

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Favorite>> Get()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);//user id from JWT
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Recipe)
                .ToListAsync();
        }

        [Authorize]
        [HttpPost("favorite")]
        public async Task<IActionResult> ToggleFavorite([FromBody] FavoriteDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var existing = await _context.Favorites
                .FirstOrDefaultAsync(f => f.RecipeId == dto.RecipeId && f.UserId == userId);

            bool added;
            if (existing != null)
            {
                _context.Favorites.Remove(existing);
                added = false;
            }
            else
            {
                _context.Favorites.Add(new Favorite { RecipeId = dto.RecipeId, UserId = userId });
                added = true;
            }

            await _context.SaveChangesAsync();
            return Ok(new { added });//zwracanie jsona "i tak", żeby nie buło pustych sytuacji i błędów nie dobrych :c
        }


    }
}
