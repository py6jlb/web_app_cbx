using System;

namespace cookbook.DTOs.Auth;

public sealed record ChangePasswordDto
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
    public required string NewPasswordConfirmation { get; set; }
}
