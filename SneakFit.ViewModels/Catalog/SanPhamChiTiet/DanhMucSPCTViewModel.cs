using SneakFit.ViewModels.Catalog.DanhMuc;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.ThuongHieu;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.SanPhamChiTiet
{
    public class DanhMucSPCTViewModel
    {
        //public List<DanhMucViewModels> DanhMucs { get; set; }
        //public List<MauSacViewModels> MauSacs { get; set; }
        //public List<ThuongHieuViewModels> ThuongHieus { get; set; }
        public IEnumerable<DanhMucViewModels> DanhMucs { get; set; }
        public IEnumerable<MauSacViewModels> MauSacs { get; set; }
        public IEnumerable<ThuongHieuViewModels> ThuongHieus { get; set; }
        public PagedResult<SanPhamViewModels> SanPhams { get; set; }
        public PagedResult<SPCTViewModels> SanPhamChiTiets { get; set; }
        public List<SPCTViewModels> AllSpct { get; set; }
        public List<SneakFit.ViewModels.Catalog.ThongKe.SanPhamChiTietThongKeViewModel> BestSellerSpct { get; set; }
        public Guid? DanhMucId { get; set; }
    }
}
