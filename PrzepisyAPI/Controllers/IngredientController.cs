using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrzepisyAPI.Db;
using PrzepisyAPI.Dtos;
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
        public async Task<ActionResult<IEnumerable<IngredientDto>>> Get()
        {
            var ingredients = await _context.Ingredients
                .Select(i => new IngredientDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Unit = i.Unit
                })
                .ToListAsync();

            return Ok(ingredients);
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
