using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.System.User
{
    public class UserViewModels
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public DateTime NgaySinh { get; set; }
        public string? UrlHinhAnh { get; set; }
        public bool TrangThai { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}
