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
            builder.Property(x => x.NgayLapHoaDon).IsRequired();
            builder.Property(x => x.TongTien).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.TrangThai).IsRequired();
            builder.Property(x => x.DiaChi).IsRequired().HasMaxLength(50);
            builder.Property(x => x.SoDienThoai).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(50);
            builder.Property(x => x.HoTen).IsRequired().HasMaxLength(50);
            builder.Property(x => x.GhiChu).HasMaxLength(200);
            builder.Property(x => x.PhuongThucThanhToan).IsRequired().HasMaxLength(50);



        }
    }
}
