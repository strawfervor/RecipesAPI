using Microsoft.AspNetCore.Authorization;
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
    public class CategoryController : ControllerBase
    {
        private readonly RecipeDbContext _context;
        public CategoryController(RecipeDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Category>> Get()
        {
            return await _context.Categories
                .Include(c => c.RecipeCategories)
                    .ThenInclude(rc => rc.Recipe)
                .ToListAsync();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Category>> Post([FromBody] Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
        }
    }

}
