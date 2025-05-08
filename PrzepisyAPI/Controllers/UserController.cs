using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserController(RecipeDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

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
        [Authorize]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? User.FindFirstValue("sub");

            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("nie można (z)autoryzować użytkownika.");
            }

            var recipe = await _context.Recipes.FindAsync(id);

            if (recipe == null)
            {
                return NotFound($"Przepis o ID {id} nie został znaleziony.");
            }

            if (recipe.UserId != userId)
            {
                return Forbid("nie możesz ususnąć tego przepisu");
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [Authorize]
        [HttpPut("{id}/change-email")]
        public async Task<IActionResult> ChangeEmail(int id, [FromBody] ChangeEmailDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Email = dto.NewEmail;
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [Authorize]
        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
                return BadRequest("Błędne hasło!");

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }

}
