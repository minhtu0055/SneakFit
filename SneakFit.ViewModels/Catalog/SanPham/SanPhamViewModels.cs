using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.SanPham
{
    public class SanPhamViewModels
    {
        public Guid Id { get; set; }
        public string TenSanPham { get; set; }
        public string Mota { get; set; }
        public Guid DanhMucId { get; set; }
        public Guid ThuongHieuId { get; set; }
        public string TenDanhMuc { get; set; }
        public string TenThuongHieu { get; set; }
        public bool TrangThai { get; set; }
    }
}
