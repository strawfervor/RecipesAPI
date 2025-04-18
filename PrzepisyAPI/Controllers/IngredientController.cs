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

        [HttpGet] public async Task<IEnumerable<Ingredient>> Get() => await _context.Ingredients.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Ingredient>> Post(Ingredient i)
        {
            _context.Ingredients.Add(i);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = i.Id }, i);
        }
    }

}
