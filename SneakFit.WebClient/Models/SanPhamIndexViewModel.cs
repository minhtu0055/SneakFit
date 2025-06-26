using SneakFit.ViewModels.Catalog.DanhMuc;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Catalog.ThuongHieu;
using SneakFit.ViewModels.Common;

namespace SneakFit.WebClient.Models
{
    public class SanPhamIndexViewModel
    {
        public PagedResult<SPCTViewModels> Products { get; set; }
        public List<DanhMucViewModels> Categories { get; set; }
        public List<MauSacViewModels> Colors { get; set; }
        public List<ThuongHieuViewModels> Brands { get; set; }
        public Guid? SelectedCategoryId { get; set; }
        public Guid? SelectedColorId { get; set; }
        public Guid? SelectedBrandId { get; set; }
    }
}
