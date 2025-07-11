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
    public class ChatLieuConfiguration : IEntityTypeConfiguration<ChatLieu>
    {
        public void Configure(EntityTypeBuilder<ChatLieu> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.TenChatLieu).HasMaxLength(50);
        }
    }
}
