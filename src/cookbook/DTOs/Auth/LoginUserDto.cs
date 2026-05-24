using System;

namespace cookbook.DTOs.Auth;

public sealed class LoginUserDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
