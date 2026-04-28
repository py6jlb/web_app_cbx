using System;

namespace cookbook.DTOs.Auth;

public sealed record RegisterUserDto
{
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required string Password { get; set; }
    public required string ConfirmationPassword { get; set; }
}
