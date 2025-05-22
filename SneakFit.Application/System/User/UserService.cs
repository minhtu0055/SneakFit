using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SneakFit.Data.Entities;
using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.User;
using static Azure.Core.HttpHeader;

namespace SneakFit.Application.System.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;

        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = configuration;
        }
        public async Task<ApiResult<string>> Authenticate(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName); // kiểm tra người dùng có tồn tại trong hệ thống hay không
            if (user == null) return new ApiErrorResult<string>("Tài khoản không tồn tại");
            if (!user.TrangThai) // kiểm tra trạng thái tài khoản
            {
                return new ApiErrorResult<string>("Tài khoản đã bị hủy kích hoạt");
            }
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true); // kiểm tra mật khẩu có đúng hay không
            if (!result.Succeeded)
            {
                return new ApiErrorResult<string>("Đăng nhập không đúng");
            }
            var roles = await _userManager.GetRolesAsync(user); // lấy danh sách quyền roles của người dùng
            // Tạo danh sách claims (thông tin nhúng vào token)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, string.Join(";",roles)),
                new Claim(ClaimTypes.Name, request.UserName),
                new Claim("SessionId", Guid.NewGuid().ToString()) // Thêm một claim để phân biệt các phiên đăng nhập
            };
            // Tạo một khóa đối xứng SymmetricSecurityKey là cùng một khóa được dùng để ký và xác minh jwt
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"])); // chuyển đổi khóa bí mật thành mảng byte[], jwt yều cầu khóa dưới dạng byte[], không phải string
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds);
            return new ApiSuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
        }
        public async Task<ApiResult<UserViewModels>> GetById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<UserViewModels>("User không tồn tại");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var userVm = new UserViewModels()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                NgaySinh = user.NgaySinh,
                TrangThai = user.TrangThai,
                Roles = roles
            };
            return new ApiSuccessResult<UserViewModels>(userVm);
        }
        public async Task<ApiResult<PagedResult<UserViewModels>>> GetUsersPaging(GetUserPagingRequest request)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(request.TuKhoa))
            {
                query = query.Where(x => x.UserName.Contains(request.TuKhoa)
                 || x.PhoneNumber.Contains(request.TuKhoa));
            }

            // Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserViewModels()
                {
                    Email = x.Email,
                    UserName = x.UserName,
                    Id = x.Id,
                    TrangThai = x.TrangThai,
                }).ToListAsync();

            // Select and projection
            var pagedResult = new PagedResult<UserViewModels>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<UserViewModels>>(pagedResult);
        }
        public async Task<ApiResult<bool>> Register(RegisterRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                return new ApiErrorResult<bool>("Tài khoản đã tồn tại");
            }
            user = new AppUser()
            {
                UserName = request.UserName,
                NgaySinh = request.NgaySinh,
                Email = request.Email,
                TrangThai = request.TrangThai,
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Đăng ký không thành công");
        }
        public async Task<bool> TrangThai(Guid id, bool trangThai)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return false;

            user.TrangThai = trangThai;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        public async Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("Tài khoản không tồn tại");
            }
            var removedRoles = request.Roles.Where(x => x.Selected == false).Select(x => x.Name).ToList();
            foreach (var roleName in removedRoles)
            {
                if (await _userManager.IsInRoleAsync(user, roleName) == true)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }
            }
            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            var addedRoles = request.Roles.Where(x => x.Selected).Select(x => x.Name).ToList();
            foreach (var roleName in addedRoles)
            {
                if (await _userManager.IsInRoleAsync(user, roleName) == false)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }

            return new ApiSuccessResult<bool>();
        }
    }
}
