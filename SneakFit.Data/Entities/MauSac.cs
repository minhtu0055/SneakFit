using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
    public class MauSac
    {
        public Guid Id { get; set; }
        public string TenMauSac { get; set; }
        public List<SanPhamChiTiet> SanPhamChiTiet { get; set; }
    }
}
