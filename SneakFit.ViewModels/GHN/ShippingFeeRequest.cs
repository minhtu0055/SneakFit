using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.GHN
{
    public class ShippingFeeRequest
    {
        public int FromDistrictId { get; set; }
        public int ToDistrictId { get; set; }
        public string ToWardCode { get; set; }
        public int Weight { get; set; } = 700;
        public int ServiceId { get; set; } = 53321;
        public int Length { get; set; } = 33;
        public int Width { get; set; } = 20;
        public int Height { get; set; } = 12;
    }
}
