using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SneakFit.Data.Configuration;
using SneakFit.Data.Entities;
using SneakFit.Data.Extensions;

namespace SneakFit.Data.EF
{
    public class SneakFitDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public SneakFitDbContext()
        {
        }

        public SneakFitDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=LAPTOP-PH9VPOUT\\SQLEXPRESS;Database=SneakFit3;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");       
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ChatLieuConfiguration());
            modelBuilder.ApplyConfiguration(new VoucherConfiguration());
            modelBuilder.ApplyConfiguration(new DanhMucConfiguration());
            modelBuilder.ApplyConfiguration(new DeGiayConfiguration());
            modelBuilder.ApplyConfiguration(new GioHangChiTietConfiguration());
            modelBuilder.ApplyConfiguration(new HinhAnhSanPhamConfiguration());
            modelBuilder.ApplyConfiguration(new HoaDonChiTietConfiguration());
            modelBuilder.ApplyConfiguration(new HoaDonConfiguration());
            modelBuilder.ApplyConfiguration(new KhuyenMaiChiTietConfiguration());
            modelBuilder.ApplyConfiguration(new KhuyenMaiConfiguration());
            modelBuilder.ApplyConfiguration(new KichThuocConfiguration());
            modelBuilder.ApplyConfiguration(new MauSacConfiguration());
            modelBuilder.ApplyConfiguration(new SanPhamChiTietConfiguration());
            modelBuilder.ApplyConfiguration(new SanPhamConfiguration());
            modelBuilder.ApplyConfiguration(new ThuongHieuConfiguration());
            modelBuilder.ApplyConfiguration(new SanPhamConfiguration());
            modelBuilder.ApplyConfiguration(new DiaChiConfiguration());
            modelBuilder.Entity<IdentityUserClaim<Guid>>();
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserLogin<Guid>>().HasKey(x => x.UserId);
            modelBuilder.Entity<IdentityRoleClaim<Guid>>();
            modelBuilder.Entity<IdentityUserToken<Guid>>().HasKey(x => x.UserId);
            modelBuilder.Seed();
        }
        public DbSet<ChatLieu> ChatLieu { get; set; }
        public DbSet<DiaChi> DiaChi { get; set; }
        public DbSet<DanhMuc> DanhMuc { get; set; }
        public DbSet<DeGiay> DeGiay { get; set; }
        public DbSet<GioHang> GioHang { get; set; }
        public DbSet<GioHangChiTiet> GioHangChiTiet { get; set; }
        public DbSet<HinhAnhSanPham> HinhAnhSanPham { get; set; }
        public DbSet<HoaDon> HoaDon { get; set; }
        public DbSet<HoaDonChiTiet> HoaDonChiTiet { get; set; }
        public DbSet<KhuyenMai> KhuyenMai { get; set; }
        public DbSet<KhuyenMaiChiTiet> KhuyenMaiChiTiet { get; set; }
        public DbSet<KichThuoc> KichThuoc { get; set; }
        public DbSet<MauSac> MauSac { get; set; }
        public DbSet<SanPham> SanPham { get; set; }
        public DbSet<SanPhamChiTiet> SanPhamChiTiet { get; set; }
        public DbSet<ThuongHieu> ThuongHieu { get; set; }
        public DbSet<Voucher> Voucher { get; set; }
        public DbSet<VoucherUser> VoucherUser { get; set; }

    }
}
