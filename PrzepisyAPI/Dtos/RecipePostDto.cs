namespace PrzepisyAPI.Dtos
{
    public class RecipePostDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Preparation { get; set; }
        public string CookingTime { get; set; }
        public string ImageUrl { get; set; }

        public List<RecipeIngredientDto> RecipeIngredients { get; set; } = new();
        public List<RecipeCategoryDto> RecipeCategories { get; set; } = new();
    }

    public class RecipeIngredientDto
    {
        public int IngredientId { get; set; }
        public float Quantity { get; set; }
    }

    public class RecipeCategoryDto
    {
        public int CategoryId { get; set; }
    }
}
