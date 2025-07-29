using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SneakFit.Application.Email;
using SneakFit.Data.EF;
using SneakFit.Data.Entities;
using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.DiaChi;
using SneakFit.ViewModels.System.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SneakFit.Application.System.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly SneakFitDbContext _context;

        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, IEmailSender emailSender, SneakFitDbContext context)
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
                new Claim(ClaimTypes.GivenName, user.HoVaTen ?? ""), 
                new Claim("SessionId", Guid.NewGuid().ToString()), // Thêm một claim để phân biệt các phiên đăng nhập
                new Claim("UrlHinhAnh", user.UrlHinhAnh ?? "") // Thêm claim cho đường dẫn ảnh
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

            // Lấy địa chỉ mặc định
            var diaChiMacDinh = await _context.DiaChi.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Mac_Dinh == true);

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
                Roles = roles,
                DiaChi = diaChiMacDinh != null ? new DiaChiViewModel
                {
                    TenDiaChi = diaChiMacDinh.TenDiaChi,
                    TenThanhPho = diaChiMacDinh.TenThanhPho,
                    TenHuyen = diaChiMacDinh.TenHuyen,
                    TenXa = diaChiMacDinh.TenXa,
                    SoDienThoai = diaChiMacDinh.SoDienThoai,
                    TenNguoiNhan = diaChiMacDinh.TenNguoiNhan,
                    MaTinh = diaChiMacDinh.MaTinh,
                    MaHuyen = diaChiMacDinh.MaHuyen,
                    MaXa = diaChiMacDinh.MaXa
                } : null
            };
            return new ApiSuccessResult<UserViewModels>(userVm);
        }
        public async Task<ApiResult<PagedResult<UserViewModels>>> GetUsersPaging(GetUserPagingRequest request)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(request.TuKhoa))
            {
                query = query.Where(x => x.HoVaTen.Contains(request.TuKhoa)
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
            // 🆕 Lọc trạng thái nếu có truyền vào
            if (request.TrangThai.HasValue)
            {
                query = query.Where(u => u.TrangThai == request.TrangThai.Value);
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
            var usernamePattern = "^[a-z0-9]+$"; // hoặc ^[a-z0-9]+$ nếu muốn cho phép số
            if (!Regex.IsMatch(request.UserName, usernamePattern))
            {
                return new ApiErrorResult<bool>("Tài khoản chỉ được chữ thường và số");
            }
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
            // Xử lý upload hình ảnh nếu có
            if (request.HinhAnh != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "users");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Tạo tên file duy nhất
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + request.HinhAnh.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.HinhAnh.CopyToAsync(fileStream);
                }

                // Cập nhật đường dẫn ảnh
                request.UrlHinhAnh = "/uploads/users/" + uniqueFileName;
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
                UrlHinhAnh = request.UrlHinhAnh
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
                    try
                    {
                        var diaChi = new Data.Entities.DiaChi
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            TenDiaChi = request.DiaChi.TenDiaChi,
                            TenThanhPho = request.DiaChi.TenThanhPho,
                            TenHuyen = request.DiaChi.TenHuyen,
                            TenXa = request.DiaChi.TenXa,
                            SoDienThoai = request.DiaChi.SoDienThoai,
                            TenNguoiNhan = request.DiaChi.TenNguoiNhan,
                            Mac_Dinh = true,
                            MaTinh = request.DiaChi.MaTinh,
                            MaHuyen = request.DiaChi.MaHuyen,
                            MaXa = request.DiaChi.MaXa
                        };
                        _context.DiaChi.Add(diaChi);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        // Nếu lỗi, xóa user vừa tạo để tránh user không có địa chỉ
                        await _userManager.DeleteAsync(user);
                        return new ApiErrorResult<bool>($"Lỗi lưu địa chỉ: {ex.Message}");
                    }
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
            // Kiểm tra email đã được sử dụng bởi user khác chưa
            var userByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userByEmail != null && userByEmail.Id != user.Id)
            {
                return new ApiErrorResult<bool>("Email đã được sử dụng");
            }

            // Kiểm tra số điện thoại đã được sử dụng bởi user khác chưa
            var userByPhone = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.SoDienThoai && x.Id != user.Id);
            if (userByPhone != null)
            {
                return new ApiErrorResult<bool>("Số điện thoại đã được sử dụng");
            }
            if (request.HinhAnh != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "users");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(user.UrlHinhAnh))
                {
                    string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.UrlHinhAnh.TrimStart('/'));
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                // Tạo tên file duy nhất
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + request.HinhAnh.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.HinhAnh.CopyToAsync(fileStream);
                }

                // Cập nhật đường dẫn ảnh
                request.UrlHinhAnh = "/uploads/users/" + uniqueFileName;
            }
            user.Email = request.Email;
            user.NgaySinh = request.NgaySinh;
            user.HoVaTen = request.HoVaTen;
            user.GioiTinh = request.GioiTinh;
            user.PhoneNumber = request.SoDienThoai;
            user.TrangThai = request.TrangThai;
            // Chỉ cập nhật UrlHinhAnh nếu có upload ảnh mới
            if (request.HinhAnh != null)
            {
                user.UrlHinhAnh = request.UrlHinhAnh;
            }

            // Cập nhật địa chỉ
            if (request.DiaChi != null)
            {
                try
                {
                    // Tìm địa chỉ hiện tại của user
                    var currentAddress = await _context.DiaChi.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Mac_Dinh == true);

                    if (currentAddress != null)
                    {
                        // Cập nhật địa chỉ hiện tại
                        currentAddress.TenDiaChi = request.DiaChi.TenDiaChi;
                        currentAddress.TenThanhPho = request.DiaChi.TenThanhPho;
                        currentAddress.TenHuyen = request.DiaChi.TenHuyen;
                        currentAddress.TenXa = request.DiaChi.TenXa;
                        currentAddress.SoDienThoai = request.DiaChi.SoDienThoai;
                        currentAddress.TenNguoiNhan = request.DiaChi.TenNguoiNhan;
                        currentAddress.MaTinh = request.DiaChi.MaTinh;
                        currentAddress.MaHuyen = request.DiaChi.MaHuyen;
                        currentAddress.MaXa = request.DiaChi.MaXa;
                    }
                    else
                    {
                        // Nếu chưa có địa chỉ, tạo mới
                        var newAddress = new Data.Entities.DiaChi
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            TenDiaChi = request.DiaChi.TenDiaChi,
                            TenThanhPho = request.DiaChi.TenThanhPho,
                            TenHuyen = request.DiaChi.TenHuyen,
                            TenXa = request.DiaChi.TenXa,
                            SoDienThoai = request.DiaChi.SoDienThoai,
                            TenNguoiNhan = request.DiaChi.TenNguoiNhan,
                            Mac_Dinh = true,
                            MaTinh = request.DiaChi.MaTinh,
                            MaHuyen = request.DiaChi.MaHuyen,
                            MaXa = request.DiaChi.MaXa
                        };
                        _context.DiaChi.Add(newAddress);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return new ApiErrorResult<bool>($"Lỗi cập nhật địa chỉ: {ex.Message}");
                }
            }

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

        public async Task<ApiResult<bool>> QuenMatKhau(string email)
        {
            try
            {
                // Tìm user bằng email
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ApiErrorResult<bool>("Email không tồn tại trong hệ thống");
                }

                // Kiểm tra trạng thái tài khoản
                if (!user.TrangThai)
                {
                    return new ApiErrorResult<bool>("Tài khoản đã bị vô hiệu hóa");
                }

                // Tạo mật khẩu mới ngẫu nhiên
                var newPassword = GenerateRandomPassword();

                // Tạo token reset password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Reset password
                var resetResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (!resetResult.Succeeded)
                {
                    return new ApiErrorResult<bool>("Lỗi đặt lại mật khẩu: " + string.Join(", ", resetResult.Errors.Select(e => e.Description)));
                }

                // Chuẩn bị nội dung email
                var emailSubject = "Mật khẩu mới cho tài khoản SneakFit của bạn";
                var emailBody = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h2>Xin chào {user.HoVaTen},</h2>
                        <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
                        <p>Mật khẩu mới của bạn là: <strong style='background-color: #f8f9fa; padding: 5px 10px; border-radius: 4px;'>{newPassword}</strong></p>
                        <p style='color: #dc3545;'><strong>Lưu ý:</strong> Vui lòng copy chính xác mật khẩu, kể cả các ký tự đặc biệt.</p>
                        <p>Vì lý do bảo mật, vui lòng đăng nhập và đổi mật khẩu ngay sau khi nhận được email này.</p>
                        <p style='color: #7f8c8d;'>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng liên hệ với chúng tôi ngay.</p>
                        <p>Trân trọng,<br>SneakFit Team</p>
                    </div>";

                // Gửi email
                await _emailSender.SendEmailAsync(user.Email, emailSubject, emailBody);

                return new ApiSuccessResult<bool>
                {
                    IsSuccessed = true,
                    Message = "Mật khẩu mới đã được gửi đến email của bạn",
                    ResultObj = true
                };
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>($"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ApiResult<bool>> DoiMatKhau(Guid id, DoiMatKhauRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("Người dùng không tồn tại");
            }

            // Kiểm tra mật khẩu hiện tại
            var isValidPassword = await _userManager.CheckPasswordAsync(user, request.MatKhauHienTai);
            if (!isValidPassword)
            {
                return new ApiErrorResult<bool>("Mật khẩu hiện tại không đúng");
            }

            // Đổi mật khẩu
            var result = await _userManager.ChangePasswordAsync(user, request.MatKhauHienTai, request.MatKhauMoi);
            if (!result.Succeeded)
            {
                return new ApiErrorResult<bool>("Đổi mật khẩu không thành công: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return new ApiSuccessResult<bool>
            {
                IsSuccessed = true,
                Message = "Đổi mật khẩu thành công",
                ResultObj = true
            };
        }
    }
}
