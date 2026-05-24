using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using cookbook.Constants;
using cookbook.DTOs.Auth;
using cookbook.Infrastructure.db;
using cookbook.Services;
using cookbook.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace cookbook.StartupExtensions;

public static class Database
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await using var identityDb = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        try
        {
            await db.Database.MigrateAsync();
            app.Logger.LogInformation("Миграции базы данных приложения применены успешно.");
            await identityDb.Database.MigrateAsync();
            app.Logger.LogInformation("Миграции базы данных identity применены успешно.");
        }
        catch (Exception e)
        {
            app.Logger.LogError(e, "Ошибка применения миграций базы данных.");
            throw;
        }
    }

    public static async Task SeedInitialData(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync(Roles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
        }

        if (!await roleManager.RoleExistsAsync(Roles.Member))
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Member));
        }
        app.Logger.LogInformation("Roles created successfully");

        var authOpts = app.Configuration.GetSection("Auth").Get<Settings.Auth>()!;

        var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
        var request = new RegisterUserDto()
        {
            Email = authOpts.AdminEmail ?? "admin@admin.ru",
            Name = "admin",
            Password = authOpts.AdminPassword ?? "!Admin12345",
            ConfirmationPassword = authOpts.AdminPassword ?? "!Admin12345",
        };

        try
        {
            var res = await authService.AddIdentityUser(request, true, true);
        }
        catch (Exception e)
        {
            app.Logger.LogError(e, "Ошибка при создании пользователя: {ErrorMessage}", e.Message);
        }
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(connectionString);
            options.UseSnakeCaseNamingConvention();
        });

        builder.Services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseSqlite(connectionString);
            options.UseSnakeCaseNamingConvention();
        });

        return builder;
    }
}
