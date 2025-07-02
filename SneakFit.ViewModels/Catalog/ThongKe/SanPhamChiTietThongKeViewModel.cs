using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.ThongKe
{
    public class SanPhamChiTietThongKeViewModel
    {
        public int STT { get; set; }
        public Guid SanPhamChiTietId { get; set; } // NÊN THÊM để thao tác chính xác từng biến thể
        public string Anh { get; set; }
        public string TenSanPham { get; set; }
        public string MauSac { get; set; }
        public string KichThuoc { get; set; }
        public string ChatLieu { get; set; }
        public string DeGiay { get; set; }
        public string ThuongHieu { get; set; }
        public decimal Gia { get; set; }
        public int SoLuongDaBan { get; set; }
        public int SoLuongConLai { get; set; }
    }
}
