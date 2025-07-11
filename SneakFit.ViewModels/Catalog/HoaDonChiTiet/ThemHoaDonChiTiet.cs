using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.HoaDonChiTiet
{
    public class ThemHoaDonChiTiet
    {
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
        public Guid HoaDonId { get; set; }
        public Guid SanPhamChiTietId { get; set; }
    }
}
