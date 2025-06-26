using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.KhuyenMai
{
    public class KhuyenMaiSPCTViewModels
    {
        public Guid SPCTId { get; set; }
        public Guid MauSacId { get; set; }
      
        public Guid KichThuocId { get; set; }
      
        public Guid ChatLieuId { get; set; }
     
        public Guid DeGiayId { get; set; }
       
        public Guid ThuongHieuId { get; set; }
       
        public Guid SanPhamId { get; set; }
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
