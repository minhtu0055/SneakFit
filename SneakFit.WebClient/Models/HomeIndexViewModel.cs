using SneakFit.ViewModels.Catalog.DanhMuc;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Catalog.KichThuoc;
using SneakFit.ViewModels.Catalog.ThuongHieu;
using SneakFit.ViewModels.Common;
using SneakFit.Data.Entities;

namespace SneakFit.WebClient.Models
{
    public class HomeIndexViewModel
    {
        public PagedResult<SPCTViewModels> Products { get; set; }
        public List<DanhMucViewModels> Categories { get; set; }
        public List<MauSacViewModels> Colors { get; set; }
        public List<KichThuocViewModels> Kichthuocs { get; set; }
        public List<ThuongHieuViewModels> Brands { get; set; }
        public Guid? SelectedCategoryId { get; set; }
        public Guid? SelectedColorId { get; set; }
        public Guid? SelectedBrandId { get; set; }
        public SanPhamViewModels SanPham { get; set; }
        public List<SanPhamViewModels> SanPhams { get; set; }
        public List<SanPhamChiTietCapNhat> ChiTietSanPhams { get; set; } = new();
        public List<SanPhamChiTiet> sanPhamChiTiets { get; set; } = new();
        public List<SPCTViewModels> SPCTviews { get; set; } = new();
    }
}
