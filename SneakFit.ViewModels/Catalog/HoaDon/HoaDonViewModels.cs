using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.HoaDon
{
    public class HoaDonViewModels
    {
        public Guid Id { get; set; }
        public string MaHoaDon { get; set; }
        public string TenKhachHang { get; set; }
        public string TenNhanVien { get; set; }
        public string LoaiDon { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal TienGiam { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
    }
}
