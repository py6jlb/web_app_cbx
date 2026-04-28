using System;
using System.Linq.Expressions;
using cookbook.Domain.Entities;
using cookbook.DTOs.Auth;

namespace cookbook.DTOs.Users;

public static class UserMapping
{
    public static Expression<Func<User, UserDto>> ProjectToDto()
    {
        return u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            Name = u.Name,
        };
    }

    public static User ToEntity(this RegisterUserDto dto)
    {
        return new User
        {
            Id = $"u_{Ulid.NewUlid()}",
            Email = dto.Email,
            Name = dto.Name,
        };
    }
}
