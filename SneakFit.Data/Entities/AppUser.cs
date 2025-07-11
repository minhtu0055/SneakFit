using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SneakFit.Data.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string? UrlHinhAnh { get; set; }  
        public DateTime NgaySinh { get; set; }
        public bool GioiTinh { get; set; }
        public bool TrangThai { get; set; }
        public string HoVaTen { get; set; }
        public List<HoaDon>? HoaDon { get; set; } 
        public List<DiaChi>? DiaChi { get; set; }
        public GioHang? GioHang { get; set; }
    }
}
