namespace PrzepisyAPI.Dtos
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Preparation { get; set; }
        public string? CookingTime { get; set; }
        public string? ImageUrl { get; set; }
        public string Author { get; set; } = string.Empty;
        public List<string> Ingredients { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public double? AverageRating { get; set; }
    }
}
