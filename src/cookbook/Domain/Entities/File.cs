using System;

namespace cookbook.Domain.Entities;

public sealed class File
{
    public string Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? MimeType { get; set; }
    public string Source { get; set; } = string.Empty;
    public long Size { get; set; }
    public bool IsTitle { get; set; }
    public string? PreviewFileId { get; set; }

    public string RecipeId { get; set; }
    public Recipe Recipe { get; set; }
    public File? PreviewFile { get; set; }
}
