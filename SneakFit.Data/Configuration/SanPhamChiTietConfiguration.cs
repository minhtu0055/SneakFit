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
    public class SanPhamChiTietConfiguration : IEntityTypeConfiguration<SanPhamChiTiet>
    {
        public void Configure(EntityTypeBuilder<SanPhamChiTiet> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.Gia);
            builder.Property(x => x.SoLuong);
            builder.Property(x => x.TrangThai);
            builder.Property(x => x.NgayTao);
            builder.HasOne(x => x.ChatLieu).WithMany(x => x.SanPhamChiTiet).HasForeignKey(x => x.ChatLieuId);
            builder.HasOne(x => x.MauSac).WithMany(x => x.SanPhamChiTiet).HasForeignKey(x => x.MauSacId);
            builder.HasOne(x => x.KichThuoc).WithMany(x => x.SanPhamChiTiet).HasForeignKey(x => x.KichThuocId);
            builder.HasOne(x => x.DeGiay).WithMany(x => x.SanPhamChiTiet).HasForeignKey(x => x.DeGiayId);
            builder.HasOne(x => x.KichThuoc).WithMany(x => x.SanPhamChiTiet).HasForeignKey(x => x.KichThuocId);
        }
    }
}
