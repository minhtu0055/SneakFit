using SneakFit.ViewModels.Catalog.DanhMuc;
using SneakFit.ViewModels.Catalog.SanPham;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.SanPhamChiTiet
{
    public class DanhMucSPCTViewModel
    {
        public List<DanhMucViewModels> DanhMucs { get; set; }
        public List<SanPhamViewModels> SanPhams { get; set; }
        public Guid? DanhMucId { get; set; }
    }
}
