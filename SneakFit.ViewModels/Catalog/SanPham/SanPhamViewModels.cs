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
        public int TongSoSanPham { get; set; }
        public string TenDanhMuc { get; set; }
        public bool TrangThai { get; set; }
        public string ImageDaiDien { get; set; }
        public decimal GiaBan { get; set; }
        public decimal GiaCu { get; set; }
    }
}
