using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
	public class SanPhamChiTiet
	{
		public Guid ID { get; set; }
		public Guid MauSacId { get; set; }
		public MauSac MauSac { get; set; }
        public Guid KichThuocId { get; set; }
		public KichThuoc KichThuoc { get; set; }
        public Guid ChatLieuId { get; set; }
		public ChatLieu ChatLieu { get; set; }
        public Guid DeGiayId { get; set; }
		public DeGiay DeGiay { get; set; }
        public Guid ThuongHieuId { get; set; }
		public ThuongHieu ThuongHieu { get; set; }
        public Guid SanPhamId { get; set; }
		public SanPham SanPham { get; set; }
		public decimal Gia { get; set; }
		public int SoLuong { get; set; }
		public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public List<GioHangChiTiet> GioHangChiTiet { get; set; }
		public List<HoaDonChiTiet> HoaDonChiTiet { get; set; }
		public List<HinhAnhSanPham> HinhAnhSanPham { get; set; }
    }
}
