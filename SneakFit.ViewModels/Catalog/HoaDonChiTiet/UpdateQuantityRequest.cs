using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.HoaDonChiTiet
{
    public class UpdateQuantityRequest
    {
        public Guid HoaDonChiTietId { get; set; }
        public int NewQuantity { get; set; }
    }
}
