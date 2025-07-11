using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SneakFit.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Configuration
{
    public class HoaDonConfiguration : IEntityTypeConfiguration<HoaDon>
    {
        public void Configure(EntityTypeBuilder<HoaDon> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.NgayTao);
            builder.Property(x => x.TongTien).HasColumnType("decimal(18,2)");
            builder.Property(x => x.TrangThai);
            builder.Property(x => x.DiaChi).HasMaxLength(50);
            builder.Property(x => x.SoDienThoai).HasMaxLength(50);
            builder.Property(x => x.Email).HasMaxLength(50);
            builder.Property(x => x.HoTen).HasMaxLength(50);
            builder.Property(x => x.GhiChu).HasMaxLength(200);
            builder.Property(x => x.PhuongThucThanhToan).HasMaxLength(50);
            builder.HasOne(x => x.User).WithMany(x => x.HoaDon).HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.Voucher).WithMany(x => x.HoaDon).HasForeignKey(x => x.VoucherId);
            builder.Property(x => x.GhiChu).HasMaxLength(200);
            builder.Property(x => x.PhuongThucThanhToan).HasMaxLength(50);
            builder.Property(x => x.VoucherId).IsRequired(false);
            // Cấu hình cho các trường mới
            builder.Property(x => x.LoaiHoaDon).HasConversion<int>();
            builder.Property(x => x.NgayThanhToan).IsRequired(false);
            builder.Property(x => x.MaHoaDon).HasMaxLength(50);
            builder.Property(x => x.PhiVanChuyen).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(x => x.DonViVanChuyen).HasMaxLength(50);
            builder.Property(x => x.MaVanDon).HasMaxLength(50);
            builder.Property(x => x.TrangThaiThanhToan).HasConversion<int>();
        }
    }
}
