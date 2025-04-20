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

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Favorite>> Post(Favorite f)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var favorite = new Favorite
            {
                UserId = userId,
                RecipeId = f.RecipeId
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = favorite.Id }, favorite);
        }
    }
}
