using System;
using cookbook.Domain.Entities;
using File = cookbook.Domain.Entities.File;

namespace cookbook.DTOs.Files;

public static class FileMappings
{
    public static FileDto ToDto(this File file)
    {
        return new FileDto
        {
            Id = file.Id,
            FileName = file.FileName,
            Source = file.Source,
            IsTitle = file.IsTitle,
            MimeType = file.MimeType,
            RecipeId = file.RecipeId,
            Size = file.Size,
            PreviewFileId = file.PreviewFileId,
        };
    }

    public static File ToEntity(this FileDto dto)
    {
        return new File
        {
            Id = $"f_{Ulid.NewUlid()}",
            FileName = dto.FileName,
            IsTitle = dto.IsTitle,
            MimeType = dto.MimeType,
            RecipeId = dto.RecipeId,
            Size = dto.Size,
            Source = dto.Source,
            PreviewFileId = dto.PreviewFileId,
        };
    }
}
