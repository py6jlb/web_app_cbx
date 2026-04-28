using System;
using Microsoft.AspNetCore.Http;

namespace cookbook.DTOs.Files;

public sealed class UploadFile
{
    public string RecipeId { get; set; }

    public IFormFile File { get; set; }

    public bool IsTitle { get; set; }

    public IFormFile? Preview { get; set; }
}
