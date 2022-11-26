using System;
using Microsoft.EntityFrameworkCore;
using GunNGoneBetter.Models;

namespace GunNGoneBetter.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Category { get; set; }
        public DbSet<MyModel> MyModel { get; set; }

        public DbSet<Product> Product { get; set; }
    }
}
