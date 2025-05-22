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
    public class KichThuocConfiguration : IEntityTypeConfiguration<KichThuoc>
    {
        public void Configure(EntityTypeBuilder<KichThuoc> builder)
        {
            builder.HasKey(k => k.Id);
            builder.Property(k => k.MaKichThuoc).IsRequired();
        }
    }
}
