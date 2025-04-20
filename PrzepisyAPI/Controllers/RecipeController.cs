using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrzepisyAPI.Db;
using PrzepisyAPI.Dtos;
using PrzepisyAPI.Models;
using System;
using System.Security.Claims;
using System.Linq;

namespace PrzepisyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly RecipeDbContext _context;
    public RecipeController(RecipeDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<RecipeDto>> Get()
    {
        return await _context.Recipes
            .Include(r => r.User)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Ratings)
            .Select(r => new RecipeDto
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Preparation = r.Preparation,
                CookingTime = r.CookingTime,
                ImageUrl = r.ImageUrl,
                Author = r.User != null ? r.User.Username : "Nieznany",
                Ingredients = r.RecipeIngredients
                    .Where(ri => ri.Ingredient != null)
                    .Select(ri => ri.Ingredient!.Name)
                    .ToList(),
                AverageRating = r.Ratings.Any() ? r.Ratings.Average(rt => rt.Score) : null
            })
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDto>> Get(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.User)
            .Include(r => r.RecipeCategories).ThenInclude(rc => rc.Category)
            .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Ratings)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null) return NotFound();

        var dto = new RecipeDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            Preparation = recipe.Preparation,
            CookingTime = recipe.CookingTime,
            ImageUrl = recipe.ImageUrl,
            Author = recipe.User?.Username ?? "Nieznany",
            Ingredients = recipe.RecipeIngredients
                .Where(ri => ri.Ingredient != null)
                .Select(ri => ri.Ingredient!.Name)
                .ToList(),
            AverageRating = recipe.Ratings.Any() ? recipe.Ratings.Average(rt => rt.Score) : null
        };

        return Ok(dto);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Recipe>> Post(Recipe recipe)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        recipe.UserId = userId;
        recipe.CreatedAt = DateTime.Now;

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
