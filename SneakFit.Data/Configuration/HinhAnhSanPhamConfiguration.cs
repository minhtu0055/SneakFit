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
    public class HinhAnhSanPhamConfiguration : IEntityTypeConfiguration<HinhAnhSanPham>
    {
        public void Configure(EntityTypeBuilder<HinhAnhSanPham> builder)
        {
            builder.HasKey(h => h.Id);
            builder.Property(h => h.UrlHinhAnh).HasMaxLength(100);
            builder.HasOne(x => x.SanPhamChiTiet).WithMany(x => x.HinhAnhSanPham).HasForeignKey(x => x.SanPhamChiTietId);
        }
    }
}
