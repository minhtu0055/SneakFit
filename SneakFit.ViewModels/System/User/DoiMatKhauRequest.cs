using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.System.User
{
    public class DoiMatKhauRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        [Display(Name = "Mật khẩu hiện tại")]
        public string MatKhauHienTai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự", MinimumLength = 6)]
        [Display(Name = "Mật khẩu mới")]
        public string MatKhauMoi { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới")]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu mới")]
        public string XacNhanMatKhau { get; set; }
    }
}
