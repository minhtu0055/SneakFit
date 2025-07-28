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
    public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
    {
        public void Configure(EntityTypeBuilder<Voucher> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.MaVoucher).HasMaxLength(50);
            builder.Property(x => x.GiaTriGiamGia).HasColumnType("decimal(18,2)");
            builder.Property(x => x.DieuKienApDung).HasColumnType("decimal(18,2)");
            builder.Property(x => x.SoLuong);
            builder.Property(x => x.NgayTao);
            builder.Property(x => x.ThoiGianBatDau);
            builder.Property(x => x.ThoiGianKetThuc);
            builder.Property(x => x.TrangThai);
            // Thêm dòng này để tránh thêm vc nhiều lần cùng 1 lúc 
            builder.HasIndex(x => x.MaVoucher).IsUnique();
        }
        public class VoucherUserConfiguration : IEntityTypeConfiguration<VoucherUser>
        {
            public void Configure(EntityTypeBuilder<VoucherUser> builder)
            {
                builder.HasKey(x => x.Id);
                builder.HasOne(x => x.Voucher)
                    .WithMany(x => x.VoucherUsers)
                    .HasForeignKey(x => x.VoucherId);
                builder.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId);
            }
        }
    }
}
