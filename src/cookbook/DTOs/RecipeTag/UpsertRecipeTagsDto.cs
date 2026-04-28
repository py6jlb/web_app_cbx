using System;

namespace cookbook.DTOs.RecipeTag;

public sealed record UpsertRecipeTagsDto
{
    public List<RecipeTagDto> Tags { get; set; }
}
