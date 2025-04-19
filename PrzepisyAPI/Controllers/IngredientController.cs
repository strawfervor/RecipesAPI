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
    public class IngredientController : ControllerBase
    {
        private readonly RecipeDbContext _context;
        public IngredientController(RecipeDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Ingredient>> Get()
        {
            return await _context.Ingredients
                .Include(i => i.RecipeIngredients)
                    .ThenInclude(ri => ri.Recipe)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Ingredient>> Post(Ingredient i)
        {
            _context.Ingredients.Add(i);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = i.Id }, i);
        }
    }

}
