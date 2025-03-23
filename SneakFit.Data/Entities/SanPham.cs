using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
	public class SanPham
	{
		public Guid Id { get; set; }
		public string TenSanPham { get; set; }
		public string Mota { get; set; }
		public bool TrangThai { get; set; }
		public Guid DanhMucId { get; set; }
	}
}
