using System;

namespace cookbook.DTOs.Recipes;

public sealed record CreateRecipeDto
{
    public string Title { get; init; }
    public string? Descriptions { get; init; }
}
