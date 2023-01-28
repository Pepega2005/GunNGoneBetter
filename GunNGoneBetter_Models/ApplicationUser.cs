using System;
using Microsoft.AspNetCore.Identity;

namespace GunNGoneBetter_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
