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
    public class RecipeController : ControllerBase
    {
        private readonly RecipeDbContext _context;
        public RecipeController(RecipeDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Recipe>> GetRecipes()
        {
            return await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Ratings)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> Get(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.RecipeCategories).ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Ratings)
                .FirstOrDefaultAsync(r => r.Id == id);

            return recipe == null ? NotFound() : Ok(recipe);
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> Post(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = recipe.Id }, recipe);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Recipe r)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) return NotFound();
            recipe.Title = r.Title;
            recipe.Description = r.Description;
            recipe.Preparation = r.Preparation;
            recipe.ImageUrl = r.ImageUrl;
            recipe.CookingTime = r.CookingTime;
            recipe.UserId = r.UserId;
            recipe.CreatedAt = r.CreatedAt;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) return NotFound();
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
