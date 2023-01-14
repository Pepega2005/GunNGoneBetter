using System;
using Microsoft.AspNetCore.Identity;

namespace GunNGoneBetter.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
