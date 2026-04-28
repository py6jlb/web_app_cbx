using System;

namespace cookbook.DTOs.Tags;

public sealed record UpdateTagDto
{
    public string Name { get; set; }
    public string Color { get; set; }
}
