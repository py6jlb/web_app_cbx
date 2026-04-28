using System;

namespace cookbook.Domain.Entities;

public class RecipeTag
{
    public string RecipeId { get; set; }
    public string TagId { get; set; }
    public string AdditionalData { get; set; }
}
