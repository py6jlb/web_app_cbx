using System;
using cookbook.Domain.Entities;
using cookbook.DTOs.Files;
using cookbook.DTOs.Recipes;
using cookbook.DTOs.Tags;
using cookbook.Services.Sorting;

namespace cookbook.DTOs.Recipes;

internal static class RecipeMapping
{
    public static Recipe ToEntity(this CreateRecipeDto dto)
    {
        return new Recipe
        {
            Created = DateTime.Now,
            Descriptions = dto.Descriptions,
            Title = dto.Title,
            Id = $"r_{Ulid.NewUlid()}",
        };
    }

    public static RecipeDto ToDto(this Recipe recipe)
    {
        // Создаём словарь для быстрого доступа к AdditionalData из RecipeTags
        var recipeTagsDict =
            recipe.RecipeTags?.ToDictionary(rt => rt.TagId, rt => rt.AdditionalData) ?? [];

        var tags = recipe.Tags?.Select(t => t.ToDto(recipeTagsDict.GetValueOrDefault(t.Id))) ?? [];
        var files = recipe.Files?.Select(x => x.ToDto()) ?? [];

        return new RecipeDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Descriptions = recipe.Descriptions,
            Created = recipe.Created,
            Tags = tags,
            Files = files,
        };
    }

    public static void UpdateFromDto(this Recipe recipe, UpdateRecipeDto dto)
    {
        recipe.Title = dto.Title;
        recipe.Descriptions = dto.Descriptions;
    }

    public static readonly SortMappingDefinition<RecipeDto, Recipe> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(RecipeDto.Title), nameof(Recipe.Title)),
            new SortMapping(nameof(RecipeDto.Descriptions), nameof(Recipe.Descriptions)),
            new SortMapping(nameof(RecipeDto.Created), nameof(Recipe.Created)),
        ],
    };
}
