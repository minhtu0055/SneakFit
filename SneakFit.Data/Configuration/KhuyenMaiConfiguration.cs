using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SneakFit.Data.Entities;

namespace SneakFit.Data.Configuration
{
    public class KhuyenMaiConfiguration : IEntityTypeConfiguration<KhuyenMai>
    {
        public void Configure(EntityTypeBuilder<KhuyenMai> builder)
        {
            builder.ToTable("KhuyenMai");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TenKhuyenMai).IsRequired().HasMaxLength(50);
            builder.Property(x => x.MoTa).HasMaxLength(200);
            builder.Property(x => x.NgayTao).IsRequired();
            builder.Property(x => x.ThoiGianBatDau).IsRequired();
            builder.Property(x => x.ThoiGianKetThuc).IsRequired();
            builder.Property(x => x.GiaTriGiamGia).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.LoaiGiamGia).IsRequired().HasConversion<int>();
            builder.Property(x => x.TrangThai).IsRequired().HasConversion<int>();
        }


    }
}
