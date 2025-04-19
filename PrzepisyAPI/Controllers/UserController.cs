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
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Recipes)
                .Include(u => u.Favorites)
                .Include(u => u.Ratings)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user == null ? NotFound() : Ok(user);
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
