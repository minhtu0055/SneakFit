using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.System.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace SneakFit.Admin.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        public LoginController(IUserApiClient userApiClient,
            IConfiguration configuration)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            var result = await _userApiClient.Authenticate(request);
            if (result.ResultObj == null)
            {
                ModelState.AddModelError("", result.Message);
                return View();
            }
            
            // Kiểm tra role của user
            var userPrincipal = this.ValidateToken(result.ResultObj);
            var roleClaim = userPrincipal.FindFirst(ClaimTypes.Role)?.Value;
            
            if (string.IsNullOrEmpty(roleClaim))
            {
                ModelState.AddModelError("", "Tài khoản không có quyền truy cập");
                return View();
            }
            
            // Chỉ cho phép Admin và Nhân Viên đăng nhập
            var allowedRoles = new[] { "Admin", "Nhân Viên" };
            var userRoles = roleClaim.Split(';');
            var hasAllowedRole = userRoles.Any(role => allowedRoles.Contains(role.Trim()));
            
            if (!hasAllowedRole)
            {
                ModelState.AddModelError("", "Bạn không có quyền truy cập vào hệ thống quản trị.");
                return View();
            }
            
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = false
            };
            HttpContext.Session.SetString("Token", result.ResultObj);
            await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal,
                        authProperties);

            return RedirectToAction("Index", "ThongKe");
        }

        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = _configuration["Tokens:Issuer"];
            validationParameters.ValidIssuer = _configuration["Tokens:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }
        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QuenMatKhau(QuenMatKhauRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["error"] = "Vui lòng nhập email hợp lệ";
                    return View(request);
                }

                if (string.IsNullOrEmpty(request?.Email))
                {
                    TempData["error"] = "Email không được để trống";
                    return View(request);
                }

                var result = await _userApiClient.QuenMatKhau(request);

                if (result == null)
                {
                    TempData["error"] = "Có lỗi xảy ra, vui lòng thử lại sau";
                    return View(request);
                }

                if (result.IsSuccessed)
                {
                    TempData["success"] = result.Message ?? "Mật khẩu mới đã được gửi vào email của bạn";
                    return RedirectToAction("Index", "Login");
                }

                TempData["error"] = result.Message ?? "Không thể gửi mật khẩu mới";
                return View(request);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Lỗi: {ex.Message}";
                return View(request);
            }
        }
    }
}
