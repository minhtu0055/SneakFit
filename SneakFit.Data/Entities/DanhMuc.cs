using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
    public class DanhMuc
    {
        public Guid Id { get; set; }
        public string TenDanhMuc { get; set; }
        public List<SanPham> SanPham { get; set; }
    }
}
