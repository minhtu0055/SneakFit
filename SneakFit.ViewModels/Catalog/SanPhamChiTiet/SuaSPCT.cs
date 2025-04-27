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

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string TenSanPham { get; set; }

        public string Mota { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        public float Gia { get; set; }

        [Required(ErrorMessage = "Số lượng sản phẩm không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [Required(ErrorMessage = "Danh mục không được để trống")]

        public Guid MauSacId { get; set; }
        public Guid KichThuocId { get; set; }
        public Guid ChatLieuId { get; set; }
        public Guid DeGiayId { get; set; }
        public Guid ThuongHieuId { get; set; }
        public Guid SanPhamId { get; set; }
        public Guid DanhMucId { get; set; }
        public bool TrangThai { get; set; } = true;

        public List<IFormFile> Images { get; set; }
    }
}
