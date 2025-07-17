using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using SneakFit.ViewModels.Catalog.HoaDonChiTietClient;
using SneakFit.ViewModels.Catalog.HoaDonClient;
using SneakFit.ViewModels.Catalog.Voucher;

namespace SneakFit.WebClient.Models
{
    public class OrderConfirmationViewModel
    {
        public HoaDonClientViewModel HoaDonClient { get; set; }
        public List<HoaDonChiTietClientViewModel> ChiTietHoaDonClient { get; set; }
        public VoucherViewModels UsedVoucher { get; set; } // Thông tin voucher đã sử dụng
    }
}
