using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.HoaDonChiTietClient
{
    public class ThemHoaDonChiTietClient
    {
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
        public Guid HoaDonId { get; set; }
        public Guid SanPhamChiTietId { get; set; }
    }
}
