using Microsoft.AspNetCore.Http;
using SneakFit.Data.Entities;
using SneakFit.ViewModels.System.DiaChi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.System.User
{
    public class RegisterRequest
    {
        public string HoVaTen { get; set; }
        public bool GioiTinh { get; set; }
        public string UserName { get; set; }
        public DateTime NgaySinh { get; set; }
        public string Email { get; set; }
        public bool TrangThai { get; set; } = true;
        public List<string> Roles { get; set; }
        public string? UrlHinhAnh { get; set; }
        public string SoDienThoai { get; set; }
        public DiaChiViewModel DiaChi { get; set; }
    }
}
