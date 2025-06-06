using Microsoft.AspNetCore.Http;
using SneakFit.ViewModels.System.DiaChi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.System.User
{
    public class UserUpdateRequest
    {
        public Guid Id { get; set; }
        public string HoVaTen { get; set; }
        public bool GioiTinh { get; set; }
        public string Email { get; set; }
        public string? SoDienThoai { get; set; } 
        public string? UrlHinhAnh { get; set; }
        public IFormFile? HinhAnh { get; set; } 
        public DateTime NgaySinh { get; set; }
        public bool TrangThai { get; set; }
        public DiaChiViewModel? DiaChi { get; set; }
    }
}
