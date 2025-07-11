using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.GioHang
{
    public class XoaSanPhamDaMuaRequest
    {
        public Guid UserId { get; set; }
        public List<Guid> SanPhamChiTietIds { get; set; }
    }
}
