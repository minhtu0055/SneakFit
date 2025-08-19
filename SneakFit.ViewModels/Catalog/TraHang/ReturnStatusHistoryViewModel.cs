using SneakFit.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.TraHang
{
    public class ReturnStatusHistoryViewModel
    {
        public Guid Id { get; set; }
        public Guid ReturnRequestId { get; set; }
        public ReturnStatus TrangThaiCu { get; set; }
        public ReturnStatus TrangThaiMoi { get; set; }
        public string GhiChu { get; set; } = string.Empty;
        public string NguoiChinhSua { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
    }
}
