using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.GioHang;
using SneakFit.ViewModels.Catalog.GioHang;
using SneakFit.ViewModels.Common;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangService _gioHangService;

        public GioHangController(IGioHangService gioHangService)
        {
            _gioHangService = gioHangService;
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GioHangPagingRequest request)
        {
            var gioHangs = await _gioHangService.GetAllPaging(request);
            return Ok(gioHangs);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var gioHang = await _gioHangService.GetById(id);
            if (gioHang == null)
                return NotFound($"Không tìm thấy giỏ hàng có id: {id}");
            return Ok(gioHang);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var gioHang = await _gioHangService.GetByUserId(userId);
            if (gioHang == null)
                return NotFound($"Không tìm thấy giỏ hàng của người dùng có id: {userId}");
            return Ok(gioHang);
        }

        [HttpPost("themvaogiohang")]
        public async Task<IActionResult> ThemVaoGioHang([FromBody] ThemVaoGioHangRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _gioHangService.ThemVaoGioHang(request);
            if (result == null)
                return BadRequest("Thêm vào giỏ hàng thất bại");
            return Ok(result);
        }

        [HttpPut("capnhat")]
        public async Task<IActionResult> CapNhatGioHang([FromBody] CapNhatGioHangRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _gioHangService.CapNhatGioHang(request);
            if (result == null)
                return BadRequest("Cập nhật giỏ hàng thất bại");
            return Ok(result);
        }

        [HttpDelete("xoasanpham/{gioHangChiTietId}")]
        public async Task<IActionResult> XoaSanPhamKhoiGioHang(Guid gioHangChiTietId)
        {
            var result = await _gioHangService.XoaSanPhamKhoiGioHang(gioHangChiTietId);
            if (!result)
                return BadRequest("Xóa sản phẩm khỏi giỏ hàng thất bại");
            return Ok(result);
        }

        [HttpPost("xoasanphamdamuakhoigiohang")]
        public async Task<IActionResult> XoaSanPhamDaMuaKhoiGioHang([FromBody] XoaSanPhamDaMuaRequest request)
        {
            if (request == null || request.SanPhamChiTietIds == null || !request.SanPhamChiTietIds.Any())
            {
                return BadRequest(new ApiErrorResult<bool>("Danh sách sản phẩm không hợp lệ"));
            }

            var result = await _gioHangService.XoaSanPhamDaMuaKhoiGioHang(request.UserId, request.SanPhamChiTietIds);
            if (result)
            {
                return Ok(new ApiSuccessResult<bool>(true));
            }
            return NotFound(new ApiErrorResult<bool>("Không tìm thấy sản phẩm hoặc giỏ hàng để xóa"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> XoaGioHang(Guid id)
        {
            var result = await _gioHangService.XoaGioHang(id);
            if (!result)
                return BadRequest("Xóa giỏ hàng thất bại");
            return Ok(result);
        }

        [HttpPost("cap-nhat-so-luong")]
        public async Task<IActionResult> CapNhatSoLuong([FromBody] CapNhatGioHang request)
        {
            var result = await _gioHangService.CapNhatSoLuongAsync(request);
            if (!result.IsSuccessed)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("tao-gio-hang-moi")]
        public async Task<IActionResult> TaoGioHangMoi([FromBody] Guid userId)
        {
            var gioHang = await _gioHangService.TaoGioHangMoi(userId);
            return Ok(gioHang);
        }
    }
}
