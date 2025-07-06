using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.KhuyenMai
{
    public class KhuyenMaiSanPhamViewModels
    {
        public Guid SanPhamId { get; set; }
        public Guid SPCTId { get; set; }
        public string TenSanPham { get; set; }       
        public decimal GiaGoc { get; set; }
        public decimal GiaKhuyenMai { get; set; }
        public string TenMauSac { get; set; }
        public string MaKichThuoc { get; set; }
        public string TenChatLieu { get; set; }
        public string TenDeGiay { get; set; }
        public string TenThuongHieu { get; set; }


    }

}
