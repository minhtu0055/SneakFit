using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.Voucher;
using System.ComponentModel.DataAnnotations;

namespace SneakFit.WebClient.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string? HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string? DiaChi { get; set; }

        [Display(Name = "Địa chỉ mới (nếu thay đổi)")]
        public string? DiaChiMoi { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public PhuongThucThanhToan? PhuongThucThanhToan { get; set; }

        public decimal PhiVanChuyen { get; set; }

        public List<GioHangItemViewModel> GioHangItems { get; set; } = new();

        public decimal TongTienSanPham { get; set; }

        public decimal? DiscountAmount { get; set; } = 0m;

        public Guid? DefaultAddressId { get; set; }

        public List<VoucherViewModels> Vouchers { get; set; } = new();

        // Add these two properties for public and private vouchers
        public List<VoucherViewModels> PublicVouchers { get; set; } = new();
        public List<VoucherViewModels> PrivateVouchers { get; set; } = new();

        public Guid? VoucherId { get; set; } // Thêm trường này để lưu id voucher đã chọn
        public CheckoutViewModel()
        {
            PhuongThucThanhToan = Data.Enums.PhuongThucThanhToan.COD;
        }
    }
}
