namespace PrzepisyAPI.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Preparation { get; set; } = "";
        public string? ImageUrl { get; set; }
        public string CookingTime { get; set; } = "";
        public int UserId { get; set; }//pełni rolę autora
        public DateTime CreatedAt { get; set; }

        //relacje
        public User User { get; set; } = new User();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<RecipeCategory> RecipeCategories { get; set; } = new List<RecipeCategory>();
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

    }
}
