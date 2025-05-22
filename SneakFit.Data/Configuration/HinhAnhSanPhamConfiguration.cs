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
            builder.ToTable("HinhAnhSanPham");
            builder.HasKey(h => h.Id);
            builder.Property(h => h.UrlHinhAnh).IsRequired().HasMaxLength(50);
        }
    }
}
