namespace PrzepisyAPI.Models
{
    public class RecipeIngredient
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = new Recipe();
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = new Ingredient();
        public float Quantity { get; set; }
    }
}
