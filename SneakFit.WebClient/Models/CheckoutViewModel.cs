using SneakFit.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace SneakFit.WebClient.Models
{
    public class CheckoutViewModel
    {
        //public string HoTen { get; set; }
        //public string SoDienThoai { get; set; }
        //public string DiaChi { get; set; }
        //public string GhiChu { get; set; }
        //public string PhuongThucThanhToan { get; set; }
        //public List<GioHangItemViewModel> GioHangItems { get; set; }
        //public decimal TongTienSanPham { get; set; }
        //public decimal TongTien { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = "";

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string DiaChi { get; set; } = "";

        [Display(Name = "Địa chỉ mới (nếu thay đổi)")]
        public string DiaChiMoi { get; set; } = "";

        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public PhuongThucThanhToan? PhuongThucThanhToan { get; set; }

        public decimal PhiVanChuyen { get; set; } = 35000;
        public List<GioHangItemViewModel> GioHangItems { get; set; } = new();
        public decimal TongTienSanPham { get; set; }
        public decimal? DiscountAmount { get; set; } = 0;
        public Guid? DefaultAddressId { get; set; }
        public CheckoutViewModel()
        {
            PhuongThucThanhToan = Data.Enums.PhuongThucThanhToan.COD;
        }
    }
}
