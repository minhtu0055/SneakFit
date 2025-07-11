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
    public class SanPhamConfiguration : IEntityTypeConfiguration<SanPham>
    {
        public void Configure(EntityTypeBuilder<SanPham> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TenSanPham).HasMaxLength(50);
            builder.Property(x => x.Mota).HasMaxLength(200);
            builder.Property(x => x.TrangThai);
            builder.HasOne(x => x.DanhMuc).WithMany(x => x.SanPham).HasForeignKey(x => x.DanhMucId);
        }
    }
}
