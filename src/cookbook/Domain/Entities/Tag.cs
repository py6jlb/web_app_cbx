namespace cookbook.Domain.Entities;

public sealed class Tag
{
    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    public List<RecipeTag> RecipeTag { get; set; }
    public List<Recipe> Recipes { get; set; }
}
