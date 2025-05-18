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
            builder.ToTable("Vouchers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.MaVoucher).IsRequired().HasMaxLength(50);
            builder.Property(x => x.GiaTriGiamGia).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.DieuKienApDung).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.SoLuong).IsRequired();
            builder.Property(x => x.NgayTao).IsRequired();
            builder.Property(x => x.ThoiGianBatDau).IsRequired();
            builder.Property(x => x.ThoiGianKetThuc).IsRequired();
            builder.Property(x => x.TrangThai).IsRequired();
        }
    }
}
