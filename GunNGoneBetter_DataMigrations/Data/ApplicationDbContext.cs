﻿using System;
using Microsoft.EntityFrameworkCore;
using GunNGoneBetter_Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GunNGoneBetter_DataMigrations.Data
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

        public DbSet<QueryHeader> QueryHeader { get; set; }
        public DbSet<QueryDetail> QueryDetail { get; set; }

        public DbSet<OrderHeader> OrderHeader { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
    }
}
