﻿namespace PrzepisyAPI.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RecipeId { get; set; }
    }
}
