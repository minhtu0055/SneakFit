using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.WebClient.Models;
using SneakFit.ViewModels.System.User;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace SneakFit.WebClient.Controllers
{

    public class LoginController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;

        public LoginController(IUserApiClient userApiClient, IConfiguration configuration)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginRegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginRegisterViewModel model, string submitType)
        {

            if (submitType == "login")
            {
                // Chỉ validate phần Login
                ModelState.Clear(); // ❗ Xóa lỗi không liên quan đến Register

                if (!TryValidateModel(model.Login))
                {
                    ViewBag.ActiveTab = "login";
                    return View(model);
                }
                var result = await _userApiClient.Authenticate(model.Login);
                if (result.ResultObj == null)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }
                var userPrincipal = this.ValidateToken(result.ResultObj);
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

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            var vm = new LoginRegisterViewModel();
            ViewBag.ActiveTab = "register";
            return View("Index", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LoginRegisterViewModel model)
        {
            ModelState.Clear();
            // Kiểm tra model hợp lệ
            if (!ModelState.IsValid)
            {
                ViewBag.ActiveTab = "register";
                return View("Index", model); // Dùng lại view Index.cshtml
            }

            var request = model.Register;
            request.Roles = new List<string> { "KHÁCH HÀNG" };

            var result = await _userApiClient.Register(request);

            if (result.IsSuccessed)
            {
                TempData["LoginSuccess"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Index");
            }

            // Nếu lỗi từ API
            if (!string.IsNullOrEmpty(result.Message))
            {
                TempData["ErrorMessage"] = result.Message;
            }

            ViewBag.ActiveTab = "register";
            return View("Index", model);
        }





        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            // Hiển thị thông tin chi tiết lỗi (PII) để dễ dàng gỡ lỗi
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken; // Khai báo biến để lưu trữ token đã được xác thực
            TokenValidationParameters validationParameters = new TokenValidationParameters(); // Tạo đối tượng chứa các tham số xác thực token
            validationParameters.ValidateLifetime = true; // Xác thực thời gian sống của token

            // Thiết lập các tham số xác thực
            validationParameters.ValidAudience = _configuration["Tokens:Issuer"]; // Đặt giá trị Audience hợp lệ từ cấu hình
            validationParameters.ValidIssuer = _configuration["Tokens:Issuer"]; // Đặt giá trị Issuer hợp lệ từ cấu hình
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"])); // Đặt khóa ký token từ cấu hình

            // Xác thực token và lấy ClaimsPrincipal
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);
            return principal; // Trả về ClaimsPrincipal sau khi xác thực token
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult QuenMatKhau()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
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
