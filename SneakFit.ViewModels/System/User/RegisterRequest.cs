using Microsoft.AspNetCore.Http;
using SneakFit.Data.Entities;
using SneakFit.ViewModels.System.DiaChi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Email phải đúng định dạng và kết thúc bằng @gmail.com")]
        public string Email { get; set; }
        public bool TrangThai { get; set; } = true;
        public List<string> Roles { get; set; }
        public string? UrlHinhAnh { get; set; }
        public IFormFile? HinhAnh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng số 0 và gồm đúng 10 chữ số, không chứa ký tự khác.")]
        public string SoDienThoai { get; set; }
        public DiaChiViewModel? DiaChi { get; set; }
    }
}
