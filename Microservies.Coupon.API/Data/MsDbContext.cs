﻿using Microsoft.EntityFrameworkCore;
using Microservices.CouponAPI.Models;

namespace Microservices.CouponAPI.Data
{
    public class MsDbContext : DbContext
    {
        public MsDbContext(DbContextOptions<MsDbContext> options) : base(options)
        {
        }

        public DbSet<CouponModel> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CouponModel>().HasData(
                new CouponModel()
                {
                    Id = 1,
                    CouponCode = "10OFF",
                    Discount = 10,
                    MinExpense = 20,
                });

            modelBuilder.Entity<CouponModel>().HasData(
                new CouponModel()
                {
                    Id = 2,
                    CouponCode = "20OFF",
                    Discount = 20,
                    MinExpense = 40,
                });
        }
    }
}