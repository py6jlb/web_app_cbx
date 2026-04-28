using System;
using cookbook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = cookbook.Domain.Entities.File;

namespace cookbook.Infrastructure.db.Configurations;

public class FileConfigurations : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.FileName).IsRequired();
        builder.Property(f => f.Source).IsRequired();

        // Связь с превью
        builder
            .HasOne(f => f.PreviewFile)
            .WithMany()
            .HasForeignKey(f => f.PreviewFileId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
