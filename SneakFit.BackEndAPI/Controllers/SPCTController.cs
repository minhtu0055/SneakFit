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

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SPCTController : ControllerBase
    {
        private readonly ISanPhamChiTetService _sanPhamChiTetService;

        public SPCTController(ISanPhamChiTetService sanPhamChiTetService)
        {
            _sanPhamChiTetService = sanPhamChiTetService;
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
                return NotFound($"Không tìm thấy sản phẩm với id = {id}");
            return Ok(item);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] ThemSPCT request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sanPham = await _sanPhamChiTetService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = sanPham.Id }, sanPham);
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromForm] SuaSPCT request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.Id = id;
            var sanPham = await _sanPhamChiTetService.Update(request);
            if (sanPham == null)
                return NotFound($"Không tìm thấy sản phẩm với id = {id}");

            return Ok(sanPham);
        }
        [HttpPatch("{id}/trangThai")]
        public async Task<IActionResult> UpdateTrangThai(Guid id, [FromBody] bool trangThai)
        {
            var result = await _sanPhamChiTetService.UpdateTrangThai(id, trangThai);
            if (!result)
                return BadRequest(new ApiErrorResult<bool>("Cập nhật trạng thái không thành công"));

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
            var product = await _sanPhamChiTetService.GetById(productId);
            if (product == null)
                return NotFound($"Không tìm thấy sản phẩm với id: {productId}");

            // Kiểm tra số lượng sau khi trừ không được âm
            if (product.SoLuong + addedQuantity < 0)
                return BadRequest("Số lượng sản phẩm trong kho không đủ");

            var result = await _sanPhamChiTetService.UpdateSoLuong(productId, product.SoLuong + addedQuantity);
            if (result)
            {
                // Lấy thông tin sản phẩm sau khi cập nhật
                var updatedProduct = await _sanPhamChiTetService.GetById(productId);
                return Ok(new
                {
                    success = true,
                    message = "Cập nhật số lượng thành công",
                    newQuantity = updatedProduct.SoLuong,
                    status = updatedProduct.TrangThai
                });
            }

            return BadRequest(new { success = false, message = "Cập nhật số lượng thất bại" });
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
    }
}
