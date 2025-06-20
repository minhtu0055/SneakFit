using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SneakFit.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Configuration
{
    public class HoaDonChiTietConfiguration : IEntityTypeConfiguration<HoaDonChiTiet>
    {
        public void Configure(EntityTypeBuilder<HoaDonChiTiet> builder)
        {          
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SoLuong);
            builder.Property(x => x.GiaBan).HasColumnType("decimal(18,2)");
            builder.HasOne(x => x.SanPhamChiTiet).WithMany(x => x.HoaDonChiTiet).HasForeignKey(x => x.SanPhamChiTietId);
            builder.HasOne(x => x.HoaDon).WithMany(x => x.HoaDonChiTiet).HasForeignKey(x => x.HoaDonId);
        }
    }
}
