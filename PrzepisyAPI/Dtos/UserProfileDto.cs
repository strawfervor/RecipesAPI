namespace PrzepisyAPI.Dtos
{
    public class UserProfileDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public int FavoritesCount { get; set; }
        public int RecipesCount { get; set; }
    }
}
