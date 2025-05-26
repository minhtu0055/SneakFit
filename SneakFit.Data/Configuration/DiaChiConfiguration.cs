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
    public class DiaChiConfiguration : IEntityTypeConfiguration<DiaChi>
    {
        public void Configure(EntityTypeBuilder<DiaChi> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TenDiaChi).IsRequired().HasMaxLength(50);
            builder.Property(x => x.TenXa).IsRequired().HasMaxLength(50);
            builder.Property(x => x.TenHuyen).IsRequired().HasMaxLength(50);
            builder.Property(x => x.TenThanhPho).IsRequired().HasMaxLength(50);
            builder.HasOne(x => x.User).WithMany(x => x.DiaChi).HasForeignKey(x => x.UserId);
        }
    }
}
