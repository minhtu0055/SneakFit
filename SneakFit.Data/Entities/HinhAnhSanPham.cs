using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
    public class HinhAnhSanPham
    {
        public Guid Id { get; set; }
        public Guid SanPhamChiTietId { get; set; }
        public SanPhamChiTiet SanPhamChiTiet { get; set; }
        public string UrlHinhAnh { get; set; }
    }
}
