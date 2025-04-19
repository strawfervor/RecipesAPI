namespace PrzepisyAPI.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Unit { get; set; } = "";

        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
}
