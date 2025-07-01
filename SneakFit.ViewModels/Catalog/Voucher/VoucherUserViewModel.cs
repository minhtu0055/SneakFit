using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.Voucher
{
    public class VoucherUserViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string HoVaTen { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public bool TrangThai { get; set; }
        public bool IsExistingUser { get; set; }
    }
}
