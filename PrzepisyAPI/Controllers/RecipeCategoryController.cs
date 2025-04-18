﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrzepisyAPI.Db;
using PrzepisyAPI.Models;
using System;

namespace PrzepisyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeCategoryController : ControllerBase
    {
        private readonly RecipeDbContext _context;
        public RecipeCategoryController(RecipeDbContext context) => _context = context;

        [HttpGet] public async Task<IEnumerable<RecipeCategory>> Get() => await _context.RecipeCategories.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<RecipeCategory>> Post(RecipeCategory rc)
        {
            _context.RecipeCategories.Add(rc);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = rc.Id }, rc);
        }
    }

}
