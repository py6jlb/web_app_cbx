using System.Security.Claims;
using cookbook.DTOs.Auth;
using cookbook.Entities.Domain;
using cookbook.Services;
using cookbook.StartupExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace cookbook.Features.Login;

public partial class Login : IRouteDefinition
{
    public IEndpointRouteBuilder MapRoutes(IEndpointRouteBuilder routes)
    {
        routes.MapGet(
            "/login",
            (HttpContext httpContext) =>
            {
                return new RazorComponentResult<Login>();
            }
        );

        routes.MapPost(
            "/login",
            async (
                [FromForm] LoginUserDto request,
                HttpContext httpContext,
                IAntiforgery antiforgery,
                IValidator<LoginUserDto> validator,
                UserManager<IdentityUser> userManager,
                ILogger<Login> logger
            ) =>
            {
                // Validate antiforgery token
                await antiforgery.ValidateRequestAsync(httpContext);

                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var emailError = validationResult
                        .Errors.FirstOrDefault(e => e.PropertyName == "Email")
                        ?.ErrorMessage;
                    var passwordError = validationResult
                        .Errors.FirstOrDefault(e => e.PropertyName == "Password")
                        ?.ErrorMessage;

                    return Results.Ok(
                        new RazorComponentResult<Login>(
                            new Dictionary<string, object?>
                            {
                                ["Email"] = request.Email,
                                ["EmailError"] = emailError,
                                ["PasswordError"] = passwordError,
                            }
                        )
                    );
                }

                try
                {
                    var identityUser = await userManager.FindByEmailAsync(request.Email);
                    if (
                        identityUser is null
                        || !await userManager.CheckPasswordAsync(identityUser, request.Password)
                    )
                    {
                        return Results.Ok(
                            new RazorComponentResult<Login>(
                                new Dictionary<string, object?>
                                {
                                    ["Email"] = request.Email,
                                    ["ErrorMessage"] = "Неверный email или пароль",
                                }
                            )
                        );
                    }

                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, identityUser.Id),
                        new(ClaimTypes.Email, identityUser.Email ?? string.Empty),
                        new(ClaimTypes.Name, identityUser.UserName ?? string.Empty),
                    };

                    var roles = await userManager.GetRolesAsync(identityUser);
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );

                    await httpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties { IsPersistent = true }
                    );

                    logger.LogInformation(
                        "Пользователь {Email} успешно вошел в систему",
                        request.Email
                    );

                    return Results.Ok(new RazorComponentResult<Home>());
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при входе пользователя {Email}", request.Email);
                    return Results.Ok(
                        new RazorComponentResult<Login>(
                            new Dictionary<string, object?>
                            {
                                ["Email"] = request.Email,
                                ["ErrorMessage"] = "Произошла ошибка при входе. Попробуйте позже",
                            }
                        )
                    );
                }
            }
        );

        return routes;
    }
}
