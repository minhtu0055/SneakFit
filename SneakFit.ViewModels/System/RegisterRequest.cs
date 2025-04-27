using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.System
{
    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime NgaySinh { get; set; }
        public string Email { get; set; }
        public bool TrangThai { get; set; } = true;
    }
}
