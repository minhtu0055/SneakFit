using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.HoaDon
{
    public class TrangThaiStep
    {
        public string Label { get; set; }
        public string Icon { get; set; } // ví dụ: "bx bx-check"
        public string Time { get; set; }
        public bool IsActive { get; set; }
    }
}
