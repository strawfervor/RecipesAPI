namespace PrzepisyAPI.Models
{
    public class RecipeCategory
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = new Recipe();
        public int CategoryId { get; set; }
        public Category Category { get; set; } = new Category();
    }
}
