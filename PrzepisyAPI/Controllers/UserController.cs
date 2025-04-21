using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrzepisyAPI.Db;
using PrzepisyAPI.Dtos;
using PrzepisyAPI.Models;
using System;
using System.Security.Claims;

namespace PrzepisyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly RecipeDbContext _context;
        public UserController(RecipeDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<User>> Get() 
        { 
            return await _context.Users
                .Include(u => u.Recipes)
                .Include(u => u.Favorites)
                .Include(u => u.Ratings)
                .ToListAsync(); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Recipes)
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            var dto = new UserProfileDto
            {
                Username = user.Username,
                Email = user.Email,
                FavoritesCount = user.Favorites?.Count ?? 0,
                RecipesCount = user.Recipes?.Count ?? 0
            };

            return Ok(dto);
        }

        [HttpGet("{id}/recipes")]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetUserRecipes(int id)
        {
            var recipes = await _context.Recipes
                .Where(r => r.UserId == id)
                .Include(r => r.RecipeCategories).ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Ratings)
                .Include(r => r.User)
                .Select(r => new RecipeDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    Preparation = r.Preparation,
                    CookingTime = r.CookingTime,
                    ImageUrl = r.ImageUrl,
                    Author = r.User.Username,
                    Ingredients = r.RecipeIngredients.Select(ri => ri.Ingredient.Name).ToList(),
                    Categories = r.RecipeCategories.Select(rc => rc.Category.Name).ToList(),
                    AverageRating = r.Ratings.Any() ? r.Ratings.Average(rt => rt.Score) : null
                })
                .ToListAsync();

            return Ok(recipes);
        }

        [HttpDelete("{id}")]
        [Authorize] // Wymaga autoryzacji (ważne!)
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            // Pobierz ID zalogowanego użytkownika z tokena JWT
            // Nazwa claimu może się różnić w zależności od konfiguracji uwierzytelniania
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) // Standardowy claim dla ID
                           ?? User.FindFirstValue("sub"); // Czasem używany w JWT

            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Nie można zidentyfikować użytkownika."); // Lub BadRequest
            }

            var recipe = await _context.Recipes.FindAsync(id);

            if (recipe == null)
            {
                return NotFound($"Przepis o ID {id} nie został znaleziony.");
            }

            // Sprawdź, czy zalogowany użytkownik jest właścicielem przepisu
            if (recipe.UserId != userId)
            {
                // Użytkownik próbuje usunąć nie swój przepis
                return Forbid("Nie masz uprawnień do usunięcia tego przepisu."); // Status 403
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            // Zwróć 204 No Content - standardowa odpowiedź dla udanego DELETE bez treści
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }
    }

}
