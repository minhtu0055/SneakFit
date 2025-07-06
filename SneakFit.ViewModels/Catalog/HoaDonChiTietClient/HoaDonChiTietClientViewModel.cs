using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.HoaDonChiTietClient
{
    public class HoaDonChiTietClientViewModel
    {
        public Guid Id { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }

        public Guid HoaDonId { get; set; }
        public Guid SanPhamChiTietId { get; set; }
        public string TenSanPham { get; set; }
        public string TenMauSac { get; set; }
        public string MaKichThuoc { get; set; }
        public string AnhSanPham { get; set; }
    }
}
