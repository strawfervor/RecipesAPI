namespace PrzepisyAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        //relacje
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
