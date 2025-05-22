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
    public class GioHangChiTietConfiguration : IEntityTypeConfiguration<GioHangChiTiet>
    {
        public void Configure(EntityTypeBuilder<GioHangChiTiet> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Gia).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.SoLuong).IsRequired();
            builder.Property(x => x.NgayTao).IsRequired();

        }
    }
}
