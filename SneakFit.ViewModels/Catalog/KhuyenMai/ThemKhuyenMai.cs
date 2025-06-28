using SneakFit.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.KhuyenMai
{
    public class ThemKhuyenMai
    {
        [Required(ErrorMessage = "Vui lòng điền vào trường này")]
        [MinLength(3, ErrorMessage = "Tên khuyến mãi phải có ít nhất 3 ký tự")]
        public string TenKhuyenMai { get; set; }

        [Required(ErrorMessage = "Vui lòng điền vào trường này")]
        public string MoTa { get; set; }

        [Required(ErrorMessage = "Vui lòng điền vào trường này")]
        public DateTime ThoiGianBatDau { get; set; }

        [Required(ErrorMessage = "Vui lòng điền vào trường này")]
        public DateTime ThoiGianKetThuc { get; set; }

        [Required(ErrorMessage = "Vui lòng điền vào trường này")]
        public LoaiGiamGia LoaiGiamGia { get; set; }

        [Required(ErrorMessage = "Vui lòng điền vào trường này")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn hoặc bằng 0")]
        public decimal GiaTriGiamGia { get; set; }
        public TrangThaiGiamGia TrangThai { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ít nhất một sản phẩm")]
        public List<Guid> SanPhamIds { get; set; }
    }
}
