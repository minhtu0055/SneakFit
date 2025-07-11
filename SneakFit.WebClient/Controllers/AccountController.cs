using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakFit.ViewModels.System.User;
using SneakFit.ApiIntegration.Services;
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

        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _userApiClient.Authenticate(model);
            if (!result.IsSuccessed)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            // Lưu token vào session
            HttpContext.Session.SetString("Token", result.ResultObj);

            // Tách JWT để lấy Id người dùng
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result.ResultObj);
            var userId = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Không thể lấy thông tin người dùng từ token.");
                return View(model);
            }

            // Gọi API lấy thông tin chi tiết user
            var userInfo = await _userApiClient.GetById(Guid.Parse(userId));
            if (!userInfo.IsSuccessed)
            {
                ModelState.AddModelError("", "Không thể lấy thông tin người dùng.");
                return View(model);
            }

            var user = userInfo.ResultObj;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.HoVaTen),
                new Claim("AvatarUrl", user.UrlHinhAnh ?? "/assets/img/default-avatar.png"),
                new Claim(ClaimTypes.Email, user.Email),
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("MyProfile");
        }

        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult MyProfile()
        {
            ViewBag.HoVaTen = User.FindFirstValue(ClaimTypes.Name) ?? "KHÁCH HÀNG";
            ViewBag.AnhDaiDien = User.FindFirstValue("AvatarUrl") ?? "/assets/img/default-avatar.png";
            return View(); // Views/Account/MyProfile.cshtml
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
