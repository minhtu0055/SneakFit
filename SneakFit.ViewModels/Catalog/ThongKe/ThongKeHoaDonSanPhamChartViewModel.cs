using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.ThongKe
{
    public class ThongKeHoaDonSanPhamChartViewModel
    {
        public List<string> Labels { get; set; } // Ngày/tháng
        public List<int> SoLuongHoaDon { get; set; }
        public List<int> SoLuongSanPham { get; set; }
    }
}
