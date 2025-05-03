using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.System.User;

namespace SneakFit.Admin.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        public UserController(IUserApiClient userApiClient,
             IConfiguration configuration)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(string tuKhoa, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetUserPagingRequest()
            {
                TuKhoa = tuKhoa,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _userApiClient.GetUsersPaging(request);
            ViewBag.TuKhoa = tuKhoa;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.ResultObj);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var result = await _userApiClient.Register(request);
            if (result.IsSuccessed)
            {
                TempData["success"] = "Thêm mới người dùng thành công";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Message);
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(Guid id, bool trangThai)
        {
            try
            {
                await _userApiClient.TrangThai(id, trangThai);
                return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
