using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.HoaDonChiTiet
{
    public class HoaDonChiTietViewModel
    {
        public Guid Id { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
        public Guid SanPhamChiTietId { get; set; }
        public string? TenSanPham { get; set; }
        public int? KichThuoc { get; set; }
        public string? MauSac { get; set; }
        public int? SoLuongTon { get; set; }
    }
}
