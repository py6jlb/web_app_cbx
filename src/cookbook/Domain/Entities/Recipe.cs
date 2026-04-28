namespace cookbook.Domain.Entities;

public sealed class Recipe
{
    public string Id { get; set; }
    public DateTime Created { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Descriptions { get; set; }

    public List<RecipeTag> RecipeTags { get; set; }
    public List<Tag> Tags { get; set; }

    public ICollection<File> Files { get; set; }
}
