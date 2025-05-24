using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.GioHang
{
    public class ThemVaoGioHangRequest
    {
        public Guid UserId { get; set; }
        public Guid SanPhamChiTietId { get; set; }
        public int SoLuong { get; set; }
    }
}
