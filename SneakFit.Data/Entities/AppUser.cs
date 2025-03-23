using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SneakFit.Data.Entities
{
    public class AppUser : IdentityUser
    {
        public string UrlHinhAnh { get; set; }  
        public DateTime NgaySinh { get; set; }
        public bool TrangThai { get; set; }
    }
}
