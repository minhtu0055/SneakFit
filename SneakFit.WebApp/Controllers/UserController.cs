using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.System.User;
using SneakFit.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using SneakFit.ViewModels.System.DiaChi;

namespace SneakFit.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : BaseController
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IRoleApiClient _roleApiClient;
        private readonly IConfiguration _configuration;
        public UserController(IUserApiClient userApiClient, IRoleApiClient roleApiClient,
             IConfiguration configuration)
        {
            _userApiClient = userApiClient;
            _roleApiClient = roleApiClient;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(string tuKhoa, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetUserPagingRequest()
            {
                TuKhoa = tuKhoa,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Role = "NHÂN VIÊN,ADMIN"
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(RegisterRequest request)
        {
            request.Roles = new List<string> { "NHÂN VIÊN" };

            var result = await _userApiClient.Register(request);
            if (result.IsSuccessed)
            {
                TempData["success"] = "Thêm mới người dùng thành công";
                return RedirectToAction("Index");
            }
            if (!string.IsNullOrEmpty(result.Message))
            {
                TempData["ErrorMessage"] = result.Message;
            }
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _userApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                var user = result.ResultObj;
                var updateRequest = new UserUpdateRequest()
                {
                    Id = id,
                    Email = user.Email,
                    GioiTinh = user.GioiTinh,
                    NgaySinh = user.NgaySinh,
                    SoDienThoai = user.SoDienThoai,
                    HoVaTen = user.HoVaTen,
                    TrangThai = user.TrangThai,
                    UrlHinhAnh = user.UrlHinhAnh,
                    DiaChi = user.DiaChi != null ? new DiaChiViewModel()
                    {
                        TenDiaChi = user.DiaChi.TenDiaChi,
                        TenThanhPho = user.DiaChi.TenThanhPho,
                        TenHuyen = user.DiaChi.TenHuyen,
                        TenXa = user.DiaChi.TenXa,
                        SoDienThoai = user.DiaChi.SoDienThoai,
                        TenNguoiNhan = user.DiaChi.TenNguoiNhan,
                        MaTinh = user.DiaChi.MaTinh,
                        MaHuyen = user.DiaChi.MaHuyen,
                        MaXa = user.DiaChi.MaXa
                    } : null
                };
                return View(updateRequest);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            // Xử lý file hình ảnh
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                if (file != null && file.Length > 0)
                {
                    request.HinhAnh = file;
                }
            }

            // Xử lý địa chỉ từ form
            if (request.DiaChi == null)
            {
                request.DiaChi = new DiaChiViewModel();
            }

            request.DiaChi.TenDiaChi = Request.Form["DiaChi.TenDiaChi"].ToString();
            request.DiaChi.TenThanhPho = Request.Form["DiaChi.TenThanhPho"].ToString();
            request.DiaChi.TenHuyen = Request.Form["DiaChi.TenHuyen"].ToString();
            request.DiaChi.TenXa = Request.Form["DiaChi.TenXa"].ToString();
            request.DiaChi.SoDienThoai = Request.Form["DiaChi.SoDienThoai"].ToString();
            request.DiaChi.TenNguoiNhan = Request.Form["DiaChi.TenNguoiNhan"].ToString();
            request.DiaChi.MaTinh = Request.Form["DiaChi.MaTinh"].ToString();
            request.DiaChi.MaHuyen = Request.Form["DiaChi.MaHuyen"].ToString();
            request.DiaChi.MaXa = Request.Form["DiaChi.MaXa"].ToString();

            var result = await _userApiClient.Update(request);
            if (result.IsSuccessed)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }
        [AllowAnonymous]
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
        [HttpGet]
        public async Task<IActionResult> RoleAssign(Guid id)
        {
            var roleAssignRequest = await GetRoleAssignRequest(id);
            return View(roleAssignRequest);
        }
        [HttpPost]
        public async Task<IActionResult> RoleAssign(RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _userApiClient.RoleAssign(request.Id, request);

            if (result.IsSuccessed)
            {
                TempData["result"] = "Cập nhật quyền thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            var roleAssignRequest = await GetRoleAssignRequest(request.Id);

            return View(roleAssignRequest);
        }
        private async Task<RoleAssignRequest> GetRoleAssignRequest(Guid id)
        {
            var userObj = await _userApiClient.GetById(id);
            var roleObj = await _roleApiClient.GetAll();
            var roleAssignRequest = new RoleAssignRequest();
            foreach (var role in roleObj.ResultObj)
            {
                roleAssignRequest.Roles.Add(new SelectItem()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Selected = userObj.ResultObj.Roles.Contains(role.Name)
                });
            }
            return roleAssignRequest;
        }
    }
}
