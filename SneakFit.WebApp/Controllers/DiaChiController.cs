using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.DiaChi;
using System.Security.Claims;

namespace SneakFit.Admin.Controllers
{
    public class DiaChiController : BaseController
    {
        private readonly IDiaChiApiClient _diaChiApiClient;
        private readonly IUserApiClient _userApiClient;

        public DiaChiController(IDiaChiApiClient diaChiApiClient, IUserApiClient userApiClient)
        {
            _diaChiApiClient = diaChiApiClient;
            _userApiClient = userApiClient;
        }

        // GET: DiaChi
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _diaChiApiClient.GetAllByUser();
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<DiaChiViewModel>());
            }
        }

        // GET: DiaChi/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var result = await _diaChiApiClient.GetById(id);
                if (result.IsSuccessed)
                {
                    return View(result.ResultObj);
                }
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: DiaChi/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DiaChi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ThemDiaChiViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var result = await _diaChiApiClient.Create(request);
                if (result.IsSuccessed)
                {
                    TempData["Success"] = "Tạo địa chỉ thành công!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = result.Message;
                return View(request);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(request);
            }
        }

        // GET: DiaChi/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var result = await _diaChiApiClient.GetById(id);
                if (result.IsSuccessed)
                {
                    var editModel = new SuaDiaChiViewModel
                    {
                        Id = result.ResultObj.Id,
                        TenDiaChi = result.ResultObj.TenDiaChi,
                        TenNguoiNhan = result.ResultObj.TenNguoiNhan,
                        SoDienThoai = result.ResultObj.SoDienThoai,
                        TenThanhPho = result.ResultObj.TenThanhPho,
                        TenHuyen = result.ResultObj.TenHuyen,
                        TenXa = result.ResultObj.TenXa,
                        MaTinh = result.ResultObj.MaTinh,
                        MaHuyen = result.ResultObj.MaHuyen,
                        MaXa = result.ResultObj.MaXa,
                        MacDinh = result.ResultObj.MacDinh
                    };
                    return View(editModel);
                }
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: DiaChi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] SuaDiaChiViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var result = await _diaChiApiClient.Update(id, request);
                if (result.IsSuccessed)
                {
                    TempData["Success"] = "Cập nhật địa chỉ thành công!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = result.Message;
                return View(request);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(request);
            }
        }

        // POST: DiaChi/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _diaChiApiClient.Delete(id);
                if (result.IsSuccessed)
                {
                    TempData["Success"] = "Xóa địa chỉ thành công!";
                }
                else
                {
                    TempData["Error"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: DiaChi/SetDefault/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(Guid id)
        {
            try
            {
                var result = await _diaChiApiClient.SetDefault(id);
                if (result.IsSuccessed)
                {
                    TempData["Success"] = "Đặt địa chỉ mặc định thành công!";
                }
                else
                {
                    TempData["Error"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UserAddressesPartial(Guid userId)
        {
            var result = await _diaChiApiClient.GetAllByUserId(userId);
            if (result.IsSuccessed)
            {
                return PartialView("_UserAddressesPartial", result.ResultObj);
            }
            return Content("Không thể lấy danh sách địa chỉ");
        }
        // GET: DiaChi/UserAddresses/{userId}
        public async Task<IActionResult> UserAddresses(Guid userId)
        {
            try
            {
                var result = await _diaChiApiClient.GetAllByUserId(userId);
                var userResult = await _userApiClient.GetById(userId);
                if (userResult.IsSuccessed)
                {
                    ViewBag.UserName = userResult.ResultObj.HoVaTen;
                }
                else
                {
                    ViewBag.UserName = "Không xác định";
                }
                ViewBag.UserId = userId;
                if (result.IsSuccessed)
                {
                    return View(result.ResultObj);
                }
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: DiaChi/CreateForUser/{userId}
        public IActionResult CreateForUser(Guid userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        // POST: DiaChi/CreateForUser/{userId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForUser(Guid userId, [FromForm] ThemDiaChiViewModel request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.UserId = userId;
                return View(request);
            }

            try
            {
                var result = await _diaChiApiClient.CreateByUser(userId, request);
                if (result.IsSuccessed)
                {
                    TempData["Success"] = "Tạo địa chỉ cho user thành công!";
                    return RedirectToAction(nameof(UserAddresses), new { userId = userId });
                }
                TempData["Error"] = result.Message;
                ViewBag.UserId = userId;
                return View(request);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.UserId = userId;
                return View(request);
            }
        }

        // POST: DiaChi/CreateForUserJson/{userId}
        [HttpPost]
        public async Task<JsonResult> CreateForUserJson(Guid userId, [FromBody] ThemDiaChiViewModel request)
        {
            if (!ModelState.IsValid)
            {
                // Trả về lỗi validate cho phía client
                return Json(new ApiResult<bool>
                {
                    IsSuccessed = false,
                    Message = "Dữ liệu không hợp lệ",
                    ResultObj = false
                });
            }

            try
            {
                var result = await _diaChiApiClient.CreateByUser(userId, request);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new ApiResult<bool>
                {
                    IsSuccessed = false,
                    Message = ex.Message,
                    ResultObj = false
                });
            }
        }

        // GET: DiaChi/EditForUser/{userId}/{id}
        public async Task<IActionResult> EditForUser(Guid userId, Guid id)
        {
            try
            {
                var result = await _diaChiApiClient.GetByIdByUser(userId, id);
                if (result.IsSuccessed)
                {
                    var editModel = new SuaDiaChiViewModel
                    {
                        Id = result.ResultObj.Id,
                        TenDiaChi = result.ResultObj.TenDiaChi,
                        TenNguoiNhan = result.ResultObj.TenNguoiNhan,
                        SoDienThoai = result.ResultObj.SoDienThoai,
                        TenThanhPho = result.ResultObj.TenThanhPho,
                        TenHuyen = result.ResultObj.TenHuyen,
                        TenXa = result.ResultObj.TenXa,
                        MaTinh = result.ResultObj.MaTinh,
                        MaHuyen = result.ResultObj.MaHuyen,
                        MaXa = result.ResultObj.MaXa,
                        MacDinh = result.ResultObj.MacDinh
                    };
                    ViewBag.UserId = userId;
                    return View(editModel);
                }
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(UserAddresses), new { userId = userId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(UserAddresses), new { userId = userId });
            }
        }

        // POST: DiaChi/EditForUser/{userId}/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditForUser(Guid userId, Guid id, [FromForm] SuaDiaChiViewModel request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.UserId = userId;
                return View(request);
            }

            try
            {
                var result = await _diaChiApiClient.UpdateByUser(userId, id, request);
                if (result.IsSuccessed)
                {
                    TempData["Success"] = "Cập nhật địa chỉ cho user thành công!";
                    return RedirectToAction(nameof(UserAddresses), new { userId = userId });
                }
                TempData["Error"] = result.Message;
                ViewBag.UserId = userId;
                return View(request);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.UserId = userId;
                return View(request);
            }
        }

        // PUT: DiaChi/EditForUserJson/{userId}/{id}
        [HttpPut]
        public async Task<JsonResult> EditForUserJson(Guid userId, Guid id, [FromBody] SneakFit.ViewModels.System.DiaChi.SuaDiaChiViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new SneakFit.ViewModels.Common.ApiResult<bool>
                {
                    IsSuccessed = false,
                    Message = "Dữ liệu không hợp lệ",
                    ResultObj = false
                });
            }

            try
            {
                var result = await _diaChiApiClient.UpdateByUser(userId, id, request);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new SneakFit.ViewModels.Common.ApiResult<bool>
                {
                    IsSuccessed = false,
                    Message = ex.Message,
                    ResultObj = false
                });
            }
        }

        // POST: DiaChi/DeleteForUser/{userId}/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteForUser(Guid userId, Guid id)
        {
            try
            {
                var result = await _diaChiApiClient.DeleteByUser(userId, id);
                if (result.IsSuccessed)
                {
                    TempData["Success"] = "Xóa địa chỉ của user thành công!";
                }
                else
                {
                    TempData["Error"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(UserAddresses), new { userId = userId });
        }

        // POST: DiaChi/SetDefaultForUser/{userId}/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefaultForUser(Guid userId, Guid id)
        {
            try
            {
                var result = await _diaChiApiClient.SetDefaultByUser(userId, id);
                if (result.IsSuccessed)
                {
                    TempData["Success"] = "Đặt địa chỉ mặc định cho user thành công!";
                }
                else
                {
                    TempData["Error"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(UserAddresses), new { userId = userId });
        }
    }
}
