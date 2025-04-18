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
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
