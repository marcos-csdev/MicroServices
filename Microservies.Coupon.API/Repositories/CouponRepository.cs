﻿using AutoMapper;
using Microservices.CouponAPI.Data;
using Microservices.CouponAPI.Models;
using Microservices.CouponAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Microservices.CouponAPI.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly MsDbContext _dbContext;
        private readonly IMapper _mapper;

        public CouponRepository(MsDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> DeleteCouponAsync(int couponId)
        {
            var coupon = await GetDbCouponByIdAsync(couponId);

            if (coupon == null) return false;

            var deletedCoupon = _dbContext.Coupons.Remove(coupon);

            await _dbContext.SaveChangesAsync();

            return deletedCoupon.State != EntityState.Unchanged;

        }
        private async Task<CouponModel?> GetDbCouponByIdAsync(int couponId)
        {
            if (couponId < 1) return null!;

            var coupon = await _dbContext.Coupons
                .FirstOrDefaultAsync(c => c.Id == couponId);

            return coupon;
        }

        public async Task<CouponDto?> GetCouponByIdAsync(int couponId)
        {
            if (couponId < 1) return null!;

            var coupon = await _dbContext.Coupons
                .FirstOrDefaultAsync(c => c.Id == couponId);

            if(coupon is null) return null!;

            var couponDto = _mapper.Map<CouponDto>(coupon);

            return couponDto;
        }

        public async Task<List<CouponDto>> GetCouponsAsync()
        {
            var dbCoupons = await _dbContext.Coupons.ToListAsync();

            if(dbCoupons == null) return new List<CouponDto>();

            var couponsDto = _mapper.Map<List<CouponDto>>(dbCoupons);

            return couponsDto;
        }

        public async Task<bool> UpsertCouponAsync(CouponDto couponDto)
        {
            if(couponDto is null) return false;

            var mappedCoupon = _mapper.Map<CouponModel>(couponDto);

            var dbCoupon = await GetDbCouponByIdAsync(mappedCoupon.Id);

            EntityEntry entityEntry;
            //Create
            if(dbCoupon is null)
            {
                entityEntry = await _dbContext.Coupons.AddAsync(mappedCoupon);
            }
            else//Update
            {
                entityEntry = _dbContext.Coupons.Update(mappedCoupon);
            }


            await _dbContext.SaveChangesAsync();

            return entityEntry.State != EntityState.Unchanged;
        }
    }
}
