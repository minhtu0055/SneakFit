using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.GioHang
{
    public class GioHangChiTietViewModel
    {
        public Guid Id { get; set; }
        public Guid GioHangId { get; set; }
        public Guid SanPhamChiTietId { get; set; }
        public string TenSanPham { get; set; }
        public string HinhAnh { get; set; }
        public string MauSac { get; set; }
        public int KichThuoc { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
