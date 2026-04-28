using System;

namespace cookbook.DTOs.Files;

public sealed record FileDto
{
    public string Id { get; init; }
    public required string FileName { get; init; }
    public string? MimeType { get; init; }
    public required string Source { get; init; }
    public long Size { get; init; }
    public bool IsTitle { get; init; }
    public string? PreviewFileId { get; init; }
    public string RecipeId { get; init; }
}
