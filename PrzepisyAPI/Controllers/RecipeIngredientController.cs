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
    public class RecipeIngredientController : ControllerBase
    {
        private readonly RecipeDbContext _context;
        public RecipeIngredientController(RecipeDbContext context) => _context = context;

        [HttpGet] public async Task<IEnumerable<RecipeIngredient>> Get() => await _context.RecipeIngredients.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<RecipeIngredient>> Post(RecipeIngredient ri)
        {
            _context.RecipeIngredients.Add(ri);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = ri.Id }, ri);
        }
    }

}
