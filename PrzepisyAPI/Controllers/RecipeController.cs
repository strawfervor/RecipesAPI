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
            .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeCategories).ThenInclude(rc => rc.Category) // ✅ DODAJ TO
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
                Categories = r.RecipeCategories
                    .Where(rc => rc.Category != null)
                    .Select(rc => rc.Category!.Name)
                    .ToList(),
                AverageRating = r.Ratings.Any() ? r.Ratings.Average(rt => rt.Score) : null
            })
            .ToListAsync();
    }

    [HttpGet("{id}/edit")]
    public async Task<ActionResult<RecipeEditDto>> GetForEdit(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeCategories).ThenInclude(rc => rc.Category)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null)
            return NotFound();

        return Ok(new RecipeEditDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            Preparation = recipe.Preparation,
            CookingTime = recipe.CookingTime,
            ImageUrl = recipe.ImageUrl,
            Ingredients = recipe.RecipeIngredients.Select(ri => new IngredientWithQuantityDto
            {
                Id = ri.Ingredient.Id,
                Name = ri.Ingredient.Name,
                Quantity = ri.Quantity
            }).ToList(),
            Categories = recipe.RecipeCategories.Select(rc => new IdNameDto
            {
                Id = rc.Category.Id,
                Name = rc.Category.Name
            }).ToList()
        });
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
    public async Task<ActionResult> Post([FromBody] RecipePostDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var recipe = new Recipe
        {
            Title = dto.Title,
            Description = dto.Description,
            Preparation = dto.Preparation,
            CookingTime = dto.CookingTime,
            ImageUrl = dto.ImageUrl,
            CreatedAt = DateTime.Now,
            UserId = userId,
            RecipeCategories = new List<RecipeCategory>(),
            RecipeIngredients = new List<RecipeIngredient>()
        };

        // Kategorie
        foreach (var categoryDto in dto.RecipeCategories)
        {
            var category = await _context.Categories.FindAsync(categoryDto.CategoryId);
            if (category != null)
            {
                recipe.RecipeCategories.Add(new RecipeCategory
                {
                    Category = category,
                    Recipe = recipe
                });
            }
        }

        // Składniki
        foreach (var ingDto in dto.RecipeIngredients)
        {
            var ingredient = await _context.Ingredients.FindAsync(ingDto.IngredientId);
            if (ingredient != null)
            {
                recipe.RecipeIngredients.Add(new RecipeIngredient
                {
                    Ingredient = ingredient,
                    Recipe = recipe,
                    Quantity = ingDto.Quantity
                });
            }
        }

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = recipe.Id }, recipe);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] RecipePostDto dto)
    {
        var recipe = await _context.Recipes
            .Include(r => r.RecipeIngredients)
            .Include(r => r.RecipeCategories)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null)
            return NotFound();

        // Podstawowe pola
        recipe.Title = dto.Title;
        recipe.Description = dto.Description;
        recipe.Preparation = dto.Preparation;
        recipe.CookingTime = dto.CookingTime;
        recipe.ImageUrl = dto.ImageUrl;

        // Kategorie
        recipe.RecipeCategories.Clear();
        foreach (var cat in dto.RecipeCategories)
        {
            var category = await _context.Categories.FindAsync(cat.CategoryId);
            if (category != null)
                recipe.RecipeCategories.Add(new RecipeCategory { Category = category });
        }

        // Składniki
        recipe.RecipeIngredients.Clear();
        foreach (var ing in dto.RecipeIngredients)
        {
            var ingredient = await _context.Ingredients.FindAsync(ing.IngredientId);
            if (ingredient != null)
                recipe.RecipeIngredients.Add(new RecipeIngredient { Ingredient = ingredient, Quantity = ing.Quantity });
        }

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
