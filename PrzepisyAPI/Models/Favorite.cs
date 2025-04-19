﻿namespace PrzepisyAPI.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = new User();
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = new Recipe();
    }
}
