namespace PrzepisyAPI.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = new User();
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = new Recipe();
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
