using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrzepisyAPI.Db;
using PrzepisyAPI.Models;
using System;

namespace PrzepisyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteController : ControllerBase
    {
        private readonly RecipeDbContext _context;
        public FavoriteController(RecipeDbContext context) => _context = context;

        [HttpGet] public async Task<IEnumerable<Favorite>> Get() => await _context.Favorites.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Favorite>> Post(Favorite f)
        {
            _context.Favorites.Add(f);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = f.Id }, f);
        }
    }

}
