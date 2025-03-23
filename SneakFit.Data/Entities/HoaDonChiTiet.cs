using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
    public class HoaDonChiTiet
    {
        public Guid Id { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
        public Guid KhuyenMaiId { get; set; }
        public Guid HoaDonId { get; set; }
        public Guid SanPhamChiTietId { get; set; }
        public  HoaDon HoaDon { get; set; }
        public SanPhamChiTiet SanPhamChiTiet { get; set; }
    }
}
