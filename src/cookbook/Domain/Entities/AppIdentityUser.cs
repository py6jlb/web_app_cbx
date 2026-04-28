using System;
using Microsoft.AspNetCore.Identity;

namespace cookbook.Entities.Domain;

public sealed class AppIdentityUser : IdentityUser
{
    public bool IsApproved { get; set; }
    public bool MustChangePassword { get; set; }
}
