using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
    public class KhuyenMaiChiTiet
    {
        public Guid Id { get; set; }
        public Guid? KhuyenMaiId { get; set; }
        public KhuyenMai KhuyenMai { get; set; }
        public Guid? SanPhamId { get; set; }
        public SanPham SanPham { get; set; }
    }
}
