using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.ThongKe
{
    public class TocDoTangTruongViewModel
    {
        public string TenChiSo { get; set; }      // Ví dụ: "Doanh thu ngày"
        public decimal GiaTriHienTai { get; set; }
        public decimal GiaTriTruocDo { get; set; }
        public double PhanTramTangTruong { get; set; } // %
        public string DonVi { get; set; }         // "VND", "Sản phẩm", "Hóa đơn"
    }
}
