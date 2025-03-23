using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
	public class SanPhamChitiet
	{
		public Guid ID { get; set; }
		public Guid MauSacId { get; set; }
		public Guid KichThuocId { get; set; }
		public Guid ChatLieuId { get; set; }
		public Guid DeGiayId { get; set; }
		public Guid ThuongHieuId { get; set; }
		public Guid SanPhamId { get; set; }

		public SanPham SanPham { get; set; }
		public float Gia { get; set; }
		public int SoLuong { get; set; }
	}
}
