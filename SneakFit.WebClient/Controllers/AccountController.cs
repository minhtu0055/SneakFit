using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakFit.ViewModels.System.User;
using SneakFit.ApiIntegration.Services;
using SneakFit.WebClient.Models;
using System.Security.Claims;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace SneakFit.WebClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserApiClient _userApiClient;

        public AccountController(IUserApiClient userApiClient)
        {
            _userApiClient = userApiClient;
        }

        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> MyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Login"); // fallback nếu session hỏng
            }

            var model = new AccountViewModel();
            var userInfo = await _userApiClient.GetById(Guid.Parse(userId));

            if (userInfo.IsSuccessed)
            {
                model.User = userInfo.ResultObj;
            }
            else
            {
                TempData["error"] = "Không thể lấy thông tin người dùng.";
            }

            return View(model); ; // Views/Account/MyProfile.cshtml
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(AccountViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    // Không cần load lại thông tin user nữa
            //    TempData["error"] = "Vui lòng kiểm tra lại thông tin nhập.";
            //    return RedirectToAction("MyProfile");
            //}

            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(uid))
            {
                TempData["error"] = "Không thể xác định người dùng.";
                return RedirectToAction("MyProfile", "Account");
            }

            var result = await _userApiClient.DoiMatKhau(Guid.Parse(uid), model.DoiMatKhauRequest);

            if (result.IsSuccessed)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Remove("Token");
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.";
                return RedirectToAction("MyProfile", "Account");
            }

            // Nếu đổi mật khẩu thất bại
            TempData["error"] = result.Message ?? "Đổi mật khẩu thất bại.";
            return RedirectToAction("MyProfile");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Index", "Login");
        }
    }
}
