using System;
using cookbook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cookbook.Infrastructure.db.Configurations;

public sealed class RecipeConfigurations : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(h => h.Id);

        builder
            .HasMany(r => r.Files)
            .WithOne(f => f.Recipe)
            .HasForeignKey(x => x.RecipeId)
            .IsRequired(false);

        builder.HasMany(h => h.Tags).WithMany().UsingEntity<RecipeTag>();

        builder.HasIndex(r => r.Title);
    }
}
