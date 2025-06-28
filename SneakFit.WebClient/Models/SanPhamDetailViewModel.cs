using SneakFit.ViewModels.Catalog.KichThuoc;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;

namespace SneakFit.WebClient.Models
{
    public class SanPhamDetailViewModel
    {
        public SanPhamViewModels SanPham { get; set; }
        public List<SPCTViewModels> SanPhamChiTiets { get; set; }
        public List<MauSacViewModels> MauSacs { get; set; }
        public List<KichThuocViewModels> KichThuocs { get; set; }
    }
}
