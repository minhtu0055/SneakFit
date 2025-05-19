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
    public class DeGiayConfiguration : IEntityTypeConfiguration<DeGiay>
    {
        public void Configure(EntityTypeBuilder<DeGiay> builder)
        {
            builder.ToTable("DeGiay");
            builder.HasKey(dg => dg.Id);
            builder.Property(dg => dg.TenDeGiay).IsRequired().HasMaxLength(50);
        }
    }
}
