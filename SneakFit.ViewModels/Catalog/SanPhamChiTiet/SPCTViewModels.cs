using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.SanPhamChiTiet
{
    public class SPCTViewModels
    {
        public Guid Id { get; set; }
        public float Gia { get; set; }
        public int SoLuong { get; set; }
        public DateTime NgayTao { get; set; }
        public Guid MauSacId { get; set; }
        public Guid KichThuocId { get; set; }
        public Guid ChatLieuId { get; set; }
        public Guid DeGiayId { get; set; }
        public Guid ThuongHieuId { get; set; }
        public Guid SanPhamId { get; set; }
        public Guid DanhMucId { get; set; }
        public bool TrangThai { get; set; }
        public string TenDanhMuc { get; set; }

        public List<string> Images { get; set; }
    }
}
