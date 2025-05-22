using SneakFit.Data.Entities;
using SneakFit.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.ConstrainedExecution;

namespace SneakFit.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DanhMuc>().HasData(
            new DanhMuc()
            {
                    Id = new Guid("8f4d4a5e-2bfa-4e8c-9d2c-3f6a7e9b87cb"),
                    TenDanhMuc = "Giày Chạy Bộ",
                },
                 new DanhMuc()
                 {
                     Id = new Guid("8f8d4a5e-2bfa-4e8c-9d2c-3f6a7e9b87cb"),
                     TenDanhMuc = "Giày Đá Bóng",
                 });
            // any guid
            var roleId = new Guid("8D04DCE2-969A-435D-BBA4-DF3F325983DC");
            var roleNVId = new Guid("8D04DCE2-969A-435D-BBA4-DF3F325984DC");
            var roleKHId = new Guid("8D04DCE2-979A-435D-BBA4-DF3F325983DC");

            var adminId = new Guid("69BD714F-9576-45BA-B5B7-F00649BE00DE");
            var nhanVienId = new Guid("69BD712F-9576-45BA-B5B7-F00649BE00DE");
            var khachHangId = new Guid("69BD714F-9576-45BA-B5B7-F01649BE00DE");
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = roleId.ToString(),
                Name = "Admin",
                NormalizedName = "Admin",
            },new IdentityRole
            {
                Id = roleNVId.ToString(),
                Name = "Nhân Viên",
                NormalizedName = "Nhân Viên",
            }, new IdentityRole
            {
                Id = roleKHId.ToString(),
                Name = "Khách Hàng",
                NormalizedName = "Khách Hàng",
            });

            var hasher = new PasswordHasher<AppUser>();
            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = adminId.ToString(),
                UserName = "Admin",
                NormalizedUserName = "Admin",
                Email = "tupmph49568@gmail.com",
                NormalizedEmail = "tupmph49568@gmail.com",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "123456aD@"),
                SecurityStamp = string.Empty,
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                RoleId = roleId,
                UserId = adminId
            });
        }
    }
}