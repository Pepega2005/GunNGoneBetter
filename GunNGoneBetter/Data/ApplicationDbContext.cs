using System;
using Microsoft.EntityFrameworkCore;
using GunNGoneBetter.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GunNGoneBetter.Data
{
    public class ApplicationDbContext: IdentityDbContext // изменили наследование
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Category { get; set; }
        public DbSet<MyModel> MyModel { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
