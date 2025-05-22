using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SneakFit.ViewModels.System.Role;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SneakFit.Application.System.Role
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<List<RoleViewModel>> GetAll()
        {
            var roles = await _roleManager.Roles
                .Select(x => new RoleViewModel()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            return roles;
        }
    }
}
