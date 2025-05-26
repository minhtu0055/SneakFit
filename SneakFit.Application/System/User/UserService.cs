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
using SneakFit.Application.Email;
using SneakFit.Data.EF;
using SneakFit.Data.Entities;
using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.User;

namespace SneakFit.Application.System.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly SneakFitDbContext _context;

        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, IEmailSender emailSender,SneakFitDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = configuration;
            _emailSender = emailSender;
            _context = context;
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
                HoVaTen = user.HoVaTen,
                UrlHinhAnh = user.UrlHinhAnh,
                SoDienThoai = user.PhoneNumber,
                GioiTinh = user.GioiTinh,
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
            // Lọc theo role nếu được chỉ định
            // if (!string.IsNullOrEmpty(request.Role))
            // {
            //     var usersInRole = await _userManager.GetUsersInRoleAsync(request.Role);
            //     var userIds = usersInRole.Select(u => u.Id);
            //     query = query.Where(u => userIds.Contains(u.Id));
            // }
            if (!string.IsNullOrEmpty(request.Role))
            {
                var roles = request.Role.Split(',');
                var userIds = new HashSet<string>();
                foreach (var role in roles)
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Trim());
                    foreach (var user in usersInRole)
                    {
                        userIds.Add(user.Id.ToString());
                    }
                }
                query = query.Where(u => userIds.Contains(u.Id.ToString()));
            }
            // Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserViewModels()
                {
                    HoVaTen = x.HoVaTen,
                    GioiTinh = x.GioiTinh,
                    NgaySinh = x.NgaySinh,
                    Email = x.Email,
                    UserName = x.UserName,
                    SoDienThoai = x.PhoneNumber,
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
            // Kiểm tra email đã tồn tại
            var userByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userByEmail != null)
            {
                return new ApiErrorResult<bool>("Email đã được sử dụng");
            }
            var userByPhone = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.SoDienThoai);
            if (userByPhone != null)
            {
                return new ApiErrorResult<bool>("Số điện thoại đã được sử dụng");
            }
            var randomPassword = GenerateRandomPassword();
            user = new AppUser()
            {
                HoVaTen = request.HoVaTen,
                GioiTinh = request.GioiTinh,
                UserName = request.UserName,
                NgaySinh = request.NgaySinh,
                PhoneNumber = request.SoDienThoai,
                Email = request.Email,
                TrangThai = request.TrangThai,
            };
            var result = await _userManager.CreateAsync(user, randomPassword);
            if (result.Succeeded)
            {
                if (request.Roles != null && request.Roles.Count > 0)
                {
                    var roleResult = await _userManager.AddToRolesAsync(user, request.Roles);
                    if (!roleResult.Succeeded)
                    {
                        return new ApiErrorResult<bool>("Thêm role không thành công");
                    }
                }
                // Thêm địa chỉ cho người dùng
                if (request.DiaChi != null)
                {
                    // Địa chỉ đầu tiên sẽ là mặc định
                    var diaChi = new Data.Entities.DiaChi
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        TenDiaChi = request.DiaChi.TenDiaChi,
                        TenThanhPho = request.DiaChi.TenThanhPho,
                        TenHuyen = request.DiaChi.TenHuyen,
                        TenXa = request.DiaChi.TenXa,
                        Mac_Dinh = true // Địa chỉ đầu tiên luôn là mặc định
                    };
                    _context.DiaChi.Add(diaChi);
                    await _context.SaveChangesAsync();
                }
                // Gửi email thông báo mật khẩu
                await _emailSender.SendEmailAsync(
                    request.Email,
                    "Thông tin tài khoản SneakFit",
                    $"Xin chào {request.HoVaTen},<br/><br/>" +
                    $"Tài khoản của bạn đã được tạo thành công trên hệ thống SneakFit.<br/>" +
                    $"Thông tin đăng nhập của bạn:<br/>" +
                    $"Tên đăng nhập: {request.UserName}<br/>" +
                    $"Mật khẩu: {randomPassword}<br/><br/>" +
                    $"Vui lòng đăng nhập và đổi mật khẩu để bảo mật tài khoản của bạn.");

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
        public async Task<ApiResult<bool>> Update(UserUpdateRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                return new ApiErrorResult<bool>("Tài khoản không tồn tại");
            user.Email = request.Email;
            user.NgaySinh = request.NgaySinh;
            user.HoVaTen = request.HoVaTen;
            user.GioiTinh = request.GioiTinh;
            user.PhoneNumber = request.SoDienThoai;
            user.TrangThai = request.TrangThai;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return new ApiSuccessResult<bool>();

            return new ApiErrorResult<bool>("Cập nhật không thành công");
        }
        private string GenerateRandomPassword()
        {
            var lowercase = "abcdefghijkmnopqrstuvwxyz";
            var uppercase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            var numbers = "23456789";
            var special = "@!#$%^&*";

            var random = new Random();
            var password = new StringBuilder();

            // Đảm bảo có ít nhất 1 ký tự từ mỗi loại
            password.Append(lowercase[random.Next(lowercase.Length)]);
            password.Append(uppercase[random.Next(uppercase.Length)]);
            password.Append(numbers[random.Next(numbers.Length)]);
            password.Append(special[random.Next(special.Length)]);

            // Thêm 4 ký tự ngẫu nhiên từ tất cả các loại
            var allChars = lowercase + uppercase + numbers + special;
            for (int i = 0; i < 4; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Trộn ngẫu nhiên các ký tự
            return new string(password.ToString().ToCharArray().OrderBy(x => random.Next()).ToArray());
        }
    }
}
