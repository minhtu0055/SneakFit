using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SneakFit.Data.Configuration;
using SneakFit.Data.Entities;

namespace SneakFit.Data.EF
{
    public class SneakFitDbContext : IdentityDbContext<AppUser>
    {
        public SneakFitDbContext()
        {
        }

        public SneakFitDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=LAPTOP-R3R9CLAI\\SQLEXPRESS;Database=SneakFit;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");       
        }
        public DbSet<ChatLieu> ChatLieu { get; set; }
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
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new AppUserConfiguration());
        }
    }
}
