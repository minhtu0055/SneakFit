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
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TenKhuyenMai).HasMaxLength(50);
            builder.Property(x => x.MoTa).HasMaxLength(200);
            builder.Property(x => x.NgayTao);
            builder.Property(x => x.ThoiGianBatDau);
            builder.Property(x => x.ThoiGianKetThuc);
            builder.Property(x => x.GiaTriGiamGia).HasColumnType("decimal(18,2)");
            builder.Property(x => x.LoaiGiamGia).HasConversion<int>();
            builder.Property(x => x.TrangThai).HasConversion<int>();

            // Thêm dòng này để tránh thêm khuyến mại nhiều lần cùng 1 lúc 
            builder.HasIndex(x => x.TenKhuyenMai).IsUnique();
        }


    }
}
