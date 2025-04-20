namespace PrzepisyAPI.Dtos
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Preparation { get; set; } = "";
        public string CookingTime { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string Author { get; set; } = "";
        public double? AverageRating { get; set; }
        public List<string> Ingredients { get; set; } = new();
    }
}
