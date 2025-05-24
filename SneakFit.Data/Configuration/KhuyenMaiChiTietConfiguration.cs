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
    public class KhuyenMaiChiTietConfiguration : IEntityTypeConfiguration<KhuyenMaiChiTiet>
    {
        public void Configure(EntityTypeBuilder<KhuyenMaiChiTiet> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.KhuyenMai).WithMany(x => x.KhuyenMaiChiTiet).HasForeignKey(x => x.KhuyenMaiId);
            builder.HasOne(x => x.SanPham).WithMany(x => x.KhuyenMaiChiTiet).HasForeignKey(x => x.SanPhamId);
        }
    }
}
