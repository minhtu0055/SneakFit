using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SneakFit.Data.Entities;

namespace SneakFit.Data.Configuration
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(x=>x.Id);
            builder.Property(u => u.UrlHinhAnh).HasMaxLength(50);
            builder.Property(x => x.UserName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.HoVaTen).HasMaxLength(50);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(50);
            builder.Property(x => x.NgaySinh).IsRequired();
            builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(50);
            builder.Property(x => x.TrangThai).IsRequired();
            builder.HasOne(x => x.GioHang).WithOne(x => x.User).HasForeignKey<GioHang>(x => x.UserId);
        }
    }
}
