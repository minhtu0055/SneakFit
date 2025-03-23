using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
    public class GioHangChiTiet
    {
        public Guid Id { get; set; }
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }
        public DateTime NgayTao { get; set; }
        public Guid GioHangId { get; set; }

        public GioHang GioHang { get; set; }
        
        public Guid SanPhamChiTietId { get; set; }
        public SanPhamChitiet SanPhamChiTiet { get; set; }
    }
}
