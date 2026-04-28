using System;
using System.Security.Cryptography;
using System.Text;
using cookbook.Constants;
using cookbook.Domain.Entities;
using cookbook.DTOs.Auth;
using cookbook.DTOs.AuthManagement;
using cookbook.DTOs.Users;
using cookbook.Entities.Domain;
using cookbook.Infrastructure.db;
using cookbook.Settings;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using AppContext = cookbook.Infrastructure.db.AppContext;

namespace cookbook.Services;

public sealed class AuthService
{
    private readonly UserManager<AppIdentityUser> _userManager;
    private readonly ILogger<AuthService> _logger;
    private readonly AppContext _db;
    private readonly AuthContext _identityDb;
    private readonly Auth _authOptions;

    public AuthService(
        ILogger<AuthService> logger,
        AppContext db,
        AuthContext identityDb,
        UserManager<AppIdentityUser> userManager,
        IOptions<Auth> authOptions
    )
    {
        _userManager = userManager;
        _db = db;
        _logger = logger;
        _identityDb = identityDb;
        _authOptions = authOptions.Value;
    }

    public async Task Login(LoginUserDto request, CancellationToken cancellationToken = default)
    {
        var identityUser = await _userManager.FindByEmailAsync(request.Email);
        if (
            identityUser is null
            || !await _userManager.CheckPasswordAsync(identityUser, request.Password)
        )
        {
            throw new Exception("Ошибка входа пользователя");
        }

        if (!identityUser.IsApproved)
        {
            throw new Exception("Учетная запись не подтверждена. Обратитесь к администратору");
        }

        if (identityUser.MustChangePassword)
        {
            throw new Exception("Необходимо сменить пароль пользователя");
        }

        var roles = await _userManager.GetRolesAsync(identityUser);
        var ct = cancellationToken;

        return;
    }

    public async Task<string> AddIdentityUser(
        RegisterUserDto request,
        bool asAdmin = false,
        bool isApproved = false,
        CancellationToken cancellationToken = default
    )
    {
        bool emailIsTaken = await _userManager.FindByEmailAsync(request.Email) is not null;
        if (emailIsTaken)
        {
            throw new Exception($"Email '{request.Email}' is already taken");
        }

        bool usernameIsTaken = await _userManager.FindByNameAsync(request.Name) is not null;

        if (usernameIsTaken)
        {
            throw new Exception($"Username '{request.Name}' is already taken");
        }

        using var transaction = await _identityDb.Database.BeginTransactionAsync(cancellationToken);
        _db.Database.SetDbConnection(_identityDb.Database.GetDbConnection());
        await _db.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

        var identityUser = new AppIdentityUser
        {
            Email = request.Email,
            UserName = request.Name,
            IsApproved = isApproved,
            MustChangePassword = !isApproved,
        };
        var identityResult = await _userManager.CreateAsync(identityUser, request.Password);
        if (!identityResult.Succeeded)
        {
            throw new Exception("Ошибка регистрации пользователя");
        }

        var addToMemberRoleResult = await _userManager.AddToRoleAsync(identityUser, Roles.Member);
        if (!addToMemberRoleResult.Succeeded)
        {
            throw new Exception("Невозможно зарегистрировать пользователя, попробуйте еще раз.");
        }

        if (asAdmin)
        {
            var addToAdminRoleResult = await _userManager.AddToRoleAsync(identityUser, Roles.Admin);
            if (!addToAdminRoleResult.Succeeded)
            {
                throw new Exception(
                    "Невозможно зарегистрировать пользователя, попробуйте еще раз."
                );
            }
        }

        var user = request.ToEntity();
        user.IdentityId = identityUser.Id;
        await _db.Users.AddAsync(user, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
        await _identityDb.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return user.Id;
    }

    public async Task ChangePassword(string userId, ChangePasswordDto request)
    {
        var identityUser = await _userManager.FindByIdAsync(userId);

        if (identityUser is null)
        {
            throw new Exception("Пользователь не найдёт.");
        }

        var passwordCheckResult = await _userManager.CheckPasswordAsync(
            identityUser,
            request.OldPassword
        );

        if (!passwordCheckResult)
        {
            throw new Exception("Неверный пароль");
        }

        var identityResult = await _userManager.ChangePasswordAsync(
            identityUser,
            request.OldPassword,
            request.NewPassword
        );

        if (!identityResult.Succeeded)
        {
            throw new Exception("Не удалось сменить пароль.");
        }
        ;

        return;
    }
}
