using System;
using cookbook.Domain.Entities;
using cookbook.DTOs.Recipes;
using cookbook.DTOs.Tags;
using cookbook.Features.Files;
using cookbook.Services;
using cookbook.Services.Sorting;
using cookbook.Settings;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection;

namespace cookbook.StartupExtensions;

public static class AppServices
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddTransient<SortMappingProvider>();
        builder.Services.Configure<Settings.Auth>(builder.Configuration.GetSection("Auth"));
        builder.Services.AddSingleton<
            ISortMappingDefinition,
            SortMappingDefinition<RecipeDto, Recipe>
        >(_ => RecipeMapping.SortMapping);

        builder.Services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<TagDto, Tag>>(
            _ => TagMappings.SortMapping
        );
        var filestorage = builder.Configuration.GetSection("Filestorage").Get<Filestorage>()!;
        Directory.CreateDirectory(filestorage.Path);

        builder.Services.Configure<Filestorage>(builder.Configuration.GetSection("Filestorage"));
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<FilesService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddRazorComponents();

        var persistence = builder.Configuration.GetSection("Persistence").Get<Persistence>()!;
        builder
            .Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(persistence.Path));

        return builder;
    }
}
