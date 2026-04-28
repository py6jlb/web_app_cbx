using System;

namespace cookbook.Settings;

public sealed record Persistence
{
    public string Path { get; init; }
}
