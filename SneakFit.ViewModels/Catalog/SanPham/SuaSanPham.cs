using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.SanPham
{
    public class SuaSanPham
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string TenSanPham { get; set; }
        public string Mota { get; set; }
        public Guid DanhMucId { get; set; }
        public bool TrangThai { get; set; }

    }
}
