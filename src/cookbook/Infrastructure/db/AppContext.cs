using System;
using cookbook.Domain.Entities;
using cookbook.Infrastructure.db.Configurations;
using Microsoft.EntityFrameworkCore;
using File = cookbook.Domain.Entities.File;

namespace cookbook.Infrastructure.db;

public sealed class AppContext : DbContext
{
    public AppContext(DbContextOptions<AppContext> options)
        : base(options) { }

    public DbSet<Tag> Tags { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<RecipeTag> RecipeTag { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new TagConfigurations());
        builder.ApplyConfiguration(new FileConfigurations());
        builder.ApplyConfiguration(new RecipeConfigurations());
        builder.ApplyConfiguration(new RecipeTagConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
    }
}
