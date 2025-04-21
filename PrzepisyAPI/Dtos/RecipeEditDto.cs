namespace PrzepisyAPI.Dtos
{
    public class RecipeEditDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? Preparation { get; set; }
        public string? CookingTime { get; set; }
        public string? ImageUrl { get; set; }

        public List<IngredientWithQuantityDto> Ingredients { get; set; }
        public List<IdNameDto> Categories { get; set; }
    }
    public class IngredientWithQuantityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public float Quantity { get; set; }
    }

    public class IdNameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}
