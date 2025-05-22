using SneakFit.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.Voucher
{
    public class UpdateVoucher
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Mã voucher không được để trống")]
        [MinLength(3, ErrorMessage = "Mã voucher phải có ít nhất 3 ký tự")]
        [MaxLength(20, ErrorMessage = "Mã voucher không được vượt quá 20 ký tự")]
        public string MaVoucher { get; set; }

        [Required(ErrorMessage = "Loại giảm giá không được để trống")]
        public LoaiGiamGia LoaiGiamGia { get; set; }

        [Required(ErrorMessage = "Giá trị giảm giá không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm giá phải lớn hơn 0")]
        public decimal GiaTriGiamGia { get; set; }

        [Required(ErrorMessage = "Điều kiện áp dụng không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Điều kiện áp dụng phải lớn hơn 0")]
        public decimal DieuKienApDung { get; set; }

        public DateTime NgayTao { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [Required(ErrorMessage = "Thời gian bắt đầu không được để trống")]
        public DateTime ThoiGianBatDau { get; set; }

        [Required(ErrorMessage = "Thời gian kết thúc không được để trống")]
        public DateTime ThoiGianKetThuc { get; set; }

        public TrangThaiGiamGia TrangThai { get; set; }
    }
}
