using System;

namespace cookbook.Settings;

public sealed record Auth
{
    public int ExpirationInMinutes { get; init; }
    public string AdminEmail { get; init; }
    public string AdminPassword { get; init; }
}
