﻿using Microsoft.AspNetCore.Identity;

namespace CodeCart.Core.Entities;

public class AppUser:IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
