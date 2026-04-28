using System;
using cookbook.Domain.Entities;
using cookbook.Services.Sorting;

namespace cookbook.DTOs.Tags;

public static class TagMappings
{
    private static readonly string[] DefaultColors =
    {
        "#E53935",
        "#D81B60",
        "#8E24AA",
        "#5E35B1",
        "#3949AB",
        "#1E88E5",
        "#039BE5",
        "#00ACC1",
        "#00897B",
        "#43A047",
        "#7CB342",
        "#C0CA33",
        "#FDD835",
        "#FFB300",
        "#FB8C00",
        "#F4511E",
        "#6D4C41",
        "#757575",
        "#546E7A",
    };

    public static Tag ToEntity(this CreateTagDto dto)
    {
        // Генерируем случайный цвет, если не передан
        var color = string.IsNullOrEmpty(dto.Color)
            ? DefaultColors[Random.Shared.Next(DefaultColors.Length)]
            : dto.Color;

        return new()
        {
            Id = $"t_{Ulid.NewUlid()}",
            Color = color,
            Name = dto.Name,
        };
    }

    public static TagDto ToDto(this Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Color = tag.Color,
            Name = tag.Name,
        };
    }

    public static TagDto ToDto(this Tag tag, string? additionalData)
    {
        return new TagDto
        {
            Id = tag.Id,
            Color = tag.Color,
            Name = tag.Name,
            AdditionalData = additionalData,
        };
    }

    public static void UpdateFromDto(this Tag tag, UpdateTagDto dto)
    {
        tag.Name = dto.Name;
        tag.Color = dto.Color;
    }

    public static readonly SortMappingDefinition<TagDto, Tag> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(TagDto.Name), nameof(Tag.Name)),
            new SortMapping(nameof(TagDto.Color), nameof(Tag.Color)),
        ],
    };
}
