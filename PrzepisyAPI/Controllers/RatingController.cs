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
    public class RatingController : ControllerBase
    {
        private readonly RecipeDbContext _context;
        public RatingController(RecipeDbContext context) => _context = context;

        [HttpGet] public async Task<IEnumerable<Rating>> Get() => await _context.Ratings.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Rating>> Post(Rating r)
        {
            _context.Ratings.Add(r);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = r.Id }, r);
        }
    }

}
