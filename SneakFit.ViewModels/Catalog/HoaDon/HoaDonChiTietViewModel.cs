using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.HoaDon
{
    public class HoaDonChiTietViewModel
    {
        public Guid Id { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
        public string SanPhamChiTietName { get; set; } // Giả định TenSanPham từ SanPhamChiTiet
    }
}
