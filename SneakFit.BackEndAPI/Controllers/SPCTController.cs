using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.SanPham;
using SneakFit.Application.Catalog.SanPhamChiTiet;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Common;
using Microsoft.Extensions.Logging;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SPCTController : ControllerBase
    {
        private readonly ISanPhamChiTetService _sanPhamChiTetService;
        private readonly ILogger<SPCTController> _logger;

        public SPCTController(ISanPhamChiTetService sanPhamChiTetService, ILogger<SPCTController> logger)
        {
            _sanPhamChiTetService = sanPhamChiTetService;
            _logger = logger;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _sanPhamChiTetService.GetAll();
            return Ok(list);
        }
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] PhanTrangSPCT request)
        {
            var products = await _sanPhamChiTetService.GetAllPaging(request);
            return Ok(new ApiSuccessResult<PagedResult<SPCTViewModels>>(products));
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _sanPhamChiTetService.GetById(id);
            if (item == null)
                return NotFound($"Không tìm thấy sản phẩm chi tiết với id = {id}");
            return Ok(item);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] ThemSPCT request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _sanPhamChiTetService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest(new { message = result.Message });

            // Xử lý nhiều hình ảnh tải lên nếu có
            if (request.Images != null && request.Images.Count > 0)
            {
                foreach (var image in request.Images)
                {
                    if (image.Length > 0)
                    {
                        var imageResult = await _sanPhamChiTetService.AddImage(result.ResultObj.Id, image);
                        if (imageResult == 0)
                        {
                            _logger.LogWarning($"Không thêm được hình ảnh cho sản phẩm {result.ResultObj.Id}");
                        }
                    }
                }
            }

            // Đảm bảo trả về URL ảnh đầy đủ
            if (result.ResultObj.Images != null && result.ResultObj.Images.Count > 0)
            {
                var baseAddress = _sanPhamChiTetService is SneakFit.Application.Catalog.SanPhamChiTiet.SanPhamChiTietService s
                    ? s.GetType().GetProperty("_baseAddress", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(s)?.ToString()
                    : null;
                result.ResultObj.Images = result.ResultObj.Images.Select(img => img.StartsWith("http") ? img : $"{baseAddress}/images/products/{img}").ToList();
            }
            return Ok(result);
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromForm] SuaSPCT request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.Id = id;
            var sanPham = await _sanPhamChiTetService.Update(request);
            if (sanPham == null)
                return NotFound($"Không tìm thấy sản phẩm chi tiết với id = {id}");

            return Ok(sanPham);
        }
        [HttpPut("{id}/trangThai")]
        public async Task<IActionResult> UpdateTrangThai(Guid id, [FromBody] bool trangThai)
        {
            var result = await _sanPhamChiTetService.UpdateTrangThai(id, trangThai);
            if (!result)
                return BadRequest(new ApiErrorResult<bool>($"Không tìm thấy sản phẩm chi tiết có ID: {id}"));

            return Ok(new ApiSuccessResult<bool>(true));
        }

        [HttpPatch("{id}/gia")]
        public async Task<IActionResult> UpdateGia(Guid id, [FromBody] decimal newPrice)
        {
            var result = await _sanPhamChiTetService.UpdateGia(id, newPrice);
            if (!result)
                return BadRequest(new ApiErrorResult<bool>("Cập nhật giá không thành công"));
            return Ok(new ApiSuccessResult<bool>(true));
        }

        [HttpPut("{productId}/soluong")]
        public async Task<IActionResult> UpdateSoLuong(Guid productId, [FromBody] int addedQuantity)
        {
            try
            {
                var product = await _sanPhamChiTetService.GetById(productId);
                if (product == null)
                    return NotFound(new ApiErrorResult<bool>($"Không tìm thấy sản phẩm với id: {productId}"));

                var result = await _sanPhamChiTetService.UpdateSoLuong(productId, addedQuantity);
                if (!result.IsSuccessed)
                    return BadRequest(result);

                var updatedProduct = await _sanPhamChiTetService.GetById(productId);
                return Ok(new ApiSuccessResult<object>(new
                {
                    success = true,
                    message = "Cập nhật số lượng thành công",
                    newQuantity = updatedProduct.SoLuong,
                    status = updatedProduct.TrangThai
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật số lượng cho sản phẩm {productId}");
                return StatusCode(500, new ApiErrorResult<bool>($"Lỗi server: {ex.Message}"));
            }
        }

        [HttpPost("{id}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddImage(Guid id, IFormFile file)
        {
            if (file == null)
                return BadRequest(new ApiErrorResult<int>("File không được để trống"));

            // Kiểm tra kích thước file (2MB)
            if (file.Length > 2 * 1024 * 1024)
                return BadRequest(new ApiErrorResult<int>("Kích thước ảnh không được vượt quá 2MB"));

            // Kiểm tra định dạng file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new ApiErrorResult<int>("Chỉ chấp nhận file ảnh có định dạng .jpg, .jpeg, .png, .gif"));

            var result = await _sanPhamChiTetService.AddImage(id, file);
            if (result == 0)
                return BadRequest(new ApiErrorResult<int>("Thêm ảnh không thành công"));

            return Ok(new ApiSuccessResult<int>(result));
        }

        [HttpDelete("images/{imageId}")]
        public async Task<IActionResult> RemoveImage(Guid imageId)
        {
            var result = await _sanPhamChiTetService.RemoveImage(imageId);
            if (result == 0)
                return BadRequest(new ApiErrorResult<int>("Xóa ảnh không thành công - ảnh không tồn tại hoặc không thể xóa"));
            return Ok(new ApiSuccessResult<int>(result));
        }

        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetListImages(Guid id)
        {
            var images = await _sanPhamChiTetService.GetListImages(id);
            return Ok(new ApiSuccessResult<List<string>>(images));
        }

        [HttpPost("CreateMultiple")]
        public async Task<IActionResult> CreateMultiple([FromBody] ThemNhieuSPCTRequest request)
        {
            if (request.Items == null || !request.Items.Any())
                return BadRequest(new ApiErrorResult<int>("Chưa chọn màu sắc/kích thước"));
            var count = await _sanPhamChiTetService.CreateMultiple(request);
            return Ok(new ApiSuccessResult<int>(count));
        }
    }
}
