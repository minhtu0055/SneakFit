using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.SanPhamChiTiet
{
    public class PhanTrangSPCT : PagingRequestBase
    {
        public string? TuKhoa { get; set; }
        public Guid? DanhMucId { get; set; }
        public Guid? MauSacId { get; set; }
        public Guid? KichThuocId { get; set; }
        public decimal? GiaThapNhat { get; set; }
        public decimal? GiaCaoNhat { get; set; }
        public bool TrangThai { get; set; }
        public bool? LocTrangthai { get; set; }
    }
}
