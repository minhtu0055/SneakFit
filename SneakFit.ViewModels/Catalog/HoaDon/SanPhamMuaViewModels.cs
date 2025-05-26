using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.HoaDon
{
    public class SanPhamMuaViewModels
    {
        public string AnhSanPham { get; set; }
        public string TenSanPham { get; set; }
        public decimal GiaBan { get; set; }
        public string KichCo { get; set; }
        public string MaMau { get; set; }   // VD: "#000000"
        public string TenMau { get; set; }  // VD: "Đen"
        public int SoLuong { get; set; }
        public decimal ThanhTien { get; set; }
        public string TrangThai { get; set; }
    }
}
