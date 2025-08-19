using SneakFit.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.TraHang
{
    public class UpdateReturnStatusRequest
    {
        public ReturnStatus NewStatus { get; set; }
        public string GhiChu { get; set; } = string.Empty;
        public string NguoiChinhSua { get; set; } = string.Empty;
    }
}
