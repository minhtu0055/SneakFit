using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDonClient;
using SneakFit.ViewModels.Catalog.TraHang;
using SneakFit.ViewModels.System.DiaChi;
using SneakFit.ViewModels.System.User;

namespace SneakFit.WebClient.Models
{
    public class AccountViewModel
    {
        public UserViewModels User { get; set; }
        public LoginRequest LoginRequest { get; set; }
        public DoiMatKhauRequest DoiMatKhauRequest { get; set; } = new DoiMatKhauRequest();
        public List<HoaDonClientViewModel> hoaDonClientViewModels { get; set; } = new();
        public Dictionary<TrangThaiHoaDon, int> SoLuongTheoTrangThai { get; set; } = new();
        public List<ReturnViewModel> returnsViewModels { get; set; } = new();
        public Dictionary<int, int> SoLuongTheoTrangThaiReturns { get; set; } = new();
        public List<DiaChiViewModel> DiaChiList { get; set; } = new();
    }
}