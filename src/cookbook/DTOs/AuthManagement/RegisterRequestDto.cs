using System;

namespace cookbook.DTOs.AuthManagement;

public sealed record NewRegisterRequestDto
{
    public required string Email { get; set; }
}
