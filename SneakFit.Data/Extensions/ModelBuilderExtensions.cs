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
                     Id = new Guid("8f8d4a6e-2bfa-4e8c-9d2c-3f6a7e9b87cb"),
                     TenDanhMuc = "Giày Đá Bóng",
                 }
            );
            modelBuilder.Entity<ChatLieu>().HasData(
                new ChatLieu()
                {
                    Id = new Guid("8f4d4a5e-2bfa-4e8c-9d2c-4f6a7e9b87cb"),
                    TenChatLieu = "Giày Chạy Bộ",
                },
                 new ChatLieu()
                 {
                     Id = new Guid("8f8d4a5e-3bfa-4e8c-9d2c-3f6a7e9b87cb"),
                     TenChatLieu = "Giày Đá Bóng",
                 }
            );
            modelBuilder.Entity<MauSac>().HasData(
                new MauSac()
                {
                    Id = new Guid("8f4d4a5e-2bfa-2e8c-9d2c-3f6a7e9b87cb"),
                    TenMauSac = "Giày Chạy Bộ",
                    MaMauSac = "#FF0000",

                },
                new MauSac()
                {
                    Id = new Guid("8f8d4a5e-2bfa-4e9c-9d2c-3f6a7e9b87cb"),
                    TenMauSac = "Giày Đá Bóng",
                    MaMauSac = "#FF0000",
                }
            );
            modelBuilder.Entity<KichThuoc>().HasData(
                new KichThuoc()
                {
                    Id = new Guid("8f4d4a6e-2bfa-4e8c-9d2c-3f6a7e9b87cb"),
                    MaKichThuoc = 40,
                },
                new KichThuoc()
                {
                    Id = new Guid("8f8d4a1e-2bfa-4e8c-9d2c-3f6a7e9b87cb"),
                    MaKichThuoc = 41,
                }
            );
            modelBuilder.Entity<DeGiay>().HasData(
                new DeGiay()
                {
                    Id = new Guid("9f4d4a6e-2bfa-4e8c-9d2c-3f6a7e9b87cb"),
                    TenDeGiay = "Cao Su",
                },
                new DeGiay()
                {
                    Id = new Guid("8f8d4a1e-2bfa-4e8c-9d2c-3f6a7e9b81cb"),
                    TenDeGiay = "Nhựa",
                }
            );
            // any guid
            var roleId = new Guid("8D04DCE2-969A-435D-BBA4-DF3F325983DC");
            var roleNVId = new Guid("8D04DCE2-969A-435D-BBA4-DF3F325984DC");
            var roleKHId = new Guid("8D04DCE2-979A-435D-BBA4-DF3F325983DC");

            var adminId = new Guid("69BD714F-9576-45BA-B5B7-F00649BE00DE");
            var nhanVienId = new Guid("69BD712F-9576-45BA-B5B7-F00649BE00DE");
            var khachHangId = new Guid("69BD714F-9576-45BA-B5B7-F01649BE00DE");
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>
                {
                    Id = roleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole<Guid>
                {
                    Id = roleNVId,
                    Name = "Nhân Viên",
                    NormalizedName = "NHÂN VIÊN"
                },
                new IdentityRole<Guid>
                {
                    Id = roleKHId,
                    Name = "Khách Hàng",
                    NormalizedName = "KHÁCH HÀNG"
                }
            );
            var hasher = new PasswordHasher<AppUser>();
            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = adminId,
                UserName = "Admin",
                HoVaTen = "Phí Minh Tú",
                NormalizedUserName = "Admin",
                Email = "tupmph49568@gmail.com",
                NormalizedEmail = "tupmph49568@gmail.com",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "123456aD@"),
                SecurityStamp = string.Empty,
                TrangThai = true
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                RoleId = roleId,
                UserId = adminId
            });
        }
    }
}