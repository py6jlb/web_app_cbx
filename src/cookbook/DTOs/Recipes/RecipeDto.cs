using System;
using cookbook.DTOs.Common;
using cookbook.DTOs.Files;
using cookbook.DTOs.Tags;

namespace cookbook.DTOs.Recipes;

public sealed record RecipesCollectionDto : ICollectionResponse<RecipeDto>
{
    public List<RecipeDto> Items { get; init; }
}

public sealed record RecipeDto
{
    public string Id { get; init; }
    public DateTime Created { get; init; }
    public string Title { get; init; }
    public string? Descriptions { get; init; }
    public IEnumerable<TagDto> Tags { get; init; }
    public IEnumerable<FileDto> Files { get; init; }
}
