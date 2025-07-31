using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.LichSuHoaDon;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Common;
using System.Globalization;
using System.Security.Claims;

namespace SneakFit.Admin.Controllers
{
   
    public class HoaDonController : BaseController
    {
        private readonly IHoaDonApiClient _hoaDonApiClient;
        private readonly IHoaDonChiTietApiClient _hoaDonChiTietApiClient;
        private readonly ISanPhamApiClient _sanPhamApiClient;

        public HoaDonController(IHoaDonApiClient hoaDonApiClient, IHoaDonChiTietApiClient hoaDonChiTietApiClient, ISanPhamApiClient sanPhamApiClient)
        {
            _hoaDonApiClient = hoaDonApiClient;
            _hoaDonChiTietApiClient = hoaDonChiTietApiClient;
            _sanPhamApiClient = sanPhamApiClient;
        }

        public async Task<IActionResult> Index(string keyword, DateTime? ngayBatDau, DateTime? ngayKetThuc, TrangThaiHoaDon? trangThai, int pageIndex = 1, int pageSize = 10)
        {
            var request = new PhanTrangHoaDon()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Trangthaihoadon = trangThai,
                NgayBatDau = ngayBatDau,
                NgayKetThuc = ngayKetThuc
            };
            var data = await _hoaDonApiClient.GetAllPaging(request);
            if (data == null)
            {
                data = new PagedResult<HoaDonViewModel>()
                {
                    Items = new List<HoaDonViewModel>(),
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecords = 0
                };
            }
            // Gọi API để lấy số lượng hóa đơn theo trạng thái
            var countByStatus = await _hoaDonApiClient.GetCountByStatusAsync();

            // Truyền dữ liệu vào ViewBag để hiển thị trong giao diện
            ViewBag.Keyword = keyword;
            ViewBag.TrangThai = trangThai;
            ViewBag.NgayBatDau = ngayBatDau;
            ViewBag.NgayKetThuc = ngayKetThuc;
            ViewBag.CountByStatus = countByStatus;

            return View(data);
        }
        public async Task<IActionResult> Details(Guid id)
        {
            var hoaDon = await _hoaDonApiClient.GetById(id);
            if (hoaDon == null)
                return NotFound();
            var chiTiet = await _hoaDonChiTietApiClient.GetByHoaDonId(id);
            hoaDon.HoaDonChiTiet = chiTiet;
            
            // Lấy lịch sử hóa đơn để hiển thị thời gian thực tế cho từng trạng thái
            var lichSuHoaDon = await _hoaDonApiClient.GetHistoryByHoaDonId(id);
            ViewBag.LichSuHoaDon = lichSuHoaDon;
            
            return View(hoaDon);
        }
        [HttpPost]
        public async Task<IActionResult> CreateHoaDonCho([FromBody] ThemHoaDon request)
        {
            try
            {
                // Thiết lập thông tin hóa đơn chờ
                request.TrangThai = TrangThaiHoaDon.ChoXacNhan;
                request.LoaiHoaDon = LoaiHoaDon.TaiQuay;


                // Gọi API để tạo hóa đơn
                var result = await _hoaDonApiClient.Create(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetHoaDonCho()
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                var hoVaTen = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.GivenName)?.Value ?? User.Identity?.Name;

                // Gọi API để lấy danh sách hóa đơn chờ của người dùng hiện tại
                var result = await _hoaDonApiClient.GetHoaDonChoByNguoiTao(hoVaTen);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteHoaDonCho(Guid id)
        {
            try
            {
                var result = await _hoaDonApiClient.Delete(id);
                if (result)
                    return Ok(new { success = true });
                return NotFound(new { success = false, message = "Không tìm thấy hóa đơn" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateHoaDonCho([FromBody] SuaHoaDon request)
        {
            try
            {
                  var result = await _hoaDonApiClient.Update(request);
                if (result != null)
                    return Ok(new { success = true, data = result });
                return NotFound(new { success = false, message = "Không tìm thấy hóa đơn" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ThanhToan([FromBody] SuaHoaDon request)
        {
            try
            {
                var result = await _hoaDonApiClient.ThanhToan(request);
                if (result)
                    return Ok(new { success = true });
                return BadRequest(new { success = false, message = "Thanh toán thất bại!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetHistoryByHoaDonId(Guid hoaDonId)
        {
            try
            {
                var result = await _hoaDonApiClient.GetHistoryByHoaDonId(hoaDonId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateHistory([FromBody] CreateLichSuHoaDonRequest request)
        {
            try
            {
                var result = await _hoaDonApiClient.CreateHistory(request);
                return Ok(new { success = true, id = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> RevertToPreviousStatus(Guid hoaDonId)
        {
            try
            {
                var result = await _hoaDonApiClient.RevertToPreviousStatus(hoaDonId);
                if (result)
                    return Ok(new { success = true });
                return BadRequest(new { success = false, message = "Không thể hoàn tác trạng thái hóa đơn!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid hoaDonId, TrangThaiHoaDon trangThai)
        {
            try
            {
                var result = await _hoaDonApiClient.UpdateStatus(hoaDonId, trangThai);
                if (result)
                    return Ok(new { success = true });
                return BadRequest(new { success = false, message = "Không thể cập nhật trạng thái hóa đơn!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatusAndLog(Guid hoaDonId, TrangThaiHoaDon newStatus, Guid userId, string? nguoiChinhSua = null, string ghiChu = null)
        {
            try
            {
                var result = await _hoaDonApiClient.UpdateStatusAndLogAsync(hoaDonId, newStatus, userId, nguoiChinhSua, ghiChu);
                if (result)
                    return Ok(new { success = true });
                return BadRequest(new { success = false, message = "Không thể cập nhật trạng thái hóa đơn!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        public IActionResult ThanhToanThanhCong()
        {
            return View();
        }
    }
}
