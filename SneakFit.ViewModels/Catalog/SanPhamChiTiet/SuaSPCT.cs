using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.SanPhamChiTiet
{
    public class SuaSPCT
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        public float Gia { get; set; }

        [Required(ErrorMessage = "Số lượng sản phẩm không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [Required(ErrorMessage = "Màu sắc không được để trống")]
        public Guid MauSacId { get; set; }
        [Required(ErrorMessage = "Kích thước không được để trống")]
        public Guid KichThuocId { get; set; }
        [Required(ErrorMessage = "Chất liệu không được để trống")]
        public Guid ChatLieuId { get; set; }
        [Required(ErrorMessage = "Đế giày không được để trống")]
        public Guid DeGiayId { get; set; }
        [Required(ErrorMessage = "Thương hiệu không được để trống")]
        public Guid ThuongHieuId { get; set; }
        [Required(ErrorMessage = "Sản phẩm không được để trống")]
        public Guid SanPhamId { get; set; }
        [Required(ErrorMessage = "Danh mục không được để trống")]
        public Guid DanhMucId { get; set; }
        public bool TrangThai { get; set; } = true;

        public List<IFormFile>? Images { get; set; }

        public List<SPCTViewModels>? DanhSachSPCT { get; set; }
    }
}
