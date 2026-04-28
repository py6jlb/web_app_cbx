using System;
using cookbook.Domain;
using cookbook.Entities.Domain;
using cookbook.Infrastructure.db;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace cookbook.StartupExtensions;

public static class Auth
{
    public static WebApplicationBuilder AddAuthenticationServices(
        this WebApplicationBuilder builder
    )
    {
        builder
            .Services.AddIdentity<AppIdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AuthContext>()
            .AddDefaultTokenProviders();

        var authOpt = builder.Configuration.GetSection("Auth").Get<Settings.Auth>()!;

        builder
            .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "cbxCookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(authOpt.ExpirationInMinutes);
                options.SlidingExpiration = true;
                options.LoginPath = "/auth/login";
                options.AccessDeniedPath = "/auth/access-denied";
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

        builder.Services.AddAuthorization();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowAll",
                policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                }
            );
        });
        return builder;
    }
}
