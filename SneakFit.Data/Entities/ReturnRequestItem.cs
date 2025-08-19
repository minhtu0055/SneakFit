using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
    public class ReturnRequestItem
    {
        public Guid Id { get; set; }
        public Guid ReturnRequestId { get; set; }
        public Guid OrderItemId { get; set; }   // HoaDonChiTiet.Id (nếu trả 1 phần)
        public int Quantity { get; set; }
        public ReturnRequest? ReturnRequest { get; set; }
    }
}
