using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.ThongKe
{
    public class TopSanPhamBanChayViewModel
    {
        public int STT { get; set; }
        public Guid SanPhamId { get; set; } // Thêm trường này
        public string TenSanPham { get; set; }
        public string MoTa { get; set; } // Thêm trường này
        public string DanhMuc { get; set; } // Thêm trường này
        public int SoLuongDaBan { get; set; }
        public int SoLuongSanPhamChiTiet { get; set; }
    }
}
