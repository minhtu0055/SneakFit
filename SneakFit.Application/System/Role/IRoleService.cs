using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SneakFit.ViewModels.System.Role;
using SneakFit.ViewModels.System.User;

namespace SneakFit.Application.System.Role
{
    public interface IRoleService
    {
        Task<List<RoleViewModel>> GetAll();
    }
}
