using System;
using cookbook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cookbook.Infrastructure.db.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasMaxLength(500);
        builder.Property(u => u.Email).HasMaxLength(500).IsRequired();
        builder.Property(u => u.Name).HasMaxLength(300);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.IdentityId).IsUnique();
    }
}
