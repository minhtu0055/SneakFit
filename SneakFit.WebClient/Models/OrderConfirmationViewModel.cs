using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using SneakFit.ViewModels.Catalog.HoaDonChiTietClient;
using SneakFit.ViewModels.Catalog.HoaDonClient;

namespace SneakFit.WebClient.Models
{
    public class OrderConfirmationViewModel
    {
        public HoaDonClientViewModel HoaDonClient { get; set; }
        public List<HoaDonChiTietClientViewModel> ChiTietHoaDonClient { get; set; }
    }
}
