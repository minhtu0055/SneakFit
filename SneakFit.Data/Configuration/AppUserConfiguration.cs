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
            builder.HasOne(x => x.GioHang).WithOne(x => x.User).HasForeignKey<GioHang>(x => x.UserId);
        }
    }
}
