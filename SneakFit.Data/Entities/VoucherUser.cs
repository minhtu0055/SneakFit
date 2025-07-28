using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
    public class VoucherUser
    {
        public Guid Id { get; set; }
        public Guid VoucherId { get; set; }
        public Guid UserId { get; set; }
        public Voucher Voucher { get; set; }
        public AppUser User { get; set; }
        public bool IsUsed { get; set; } = false;

    }
}