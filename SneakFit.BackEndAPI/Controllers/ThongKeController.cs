using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.ThongKe;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKeController : ControllerBase
    {
        private readonly IThongKeService _thongKeService;
        public ThongKeController(IThongKeService thongKeService)
        {
            _thongKeService = thongKeService;
        }

        [HttpGet("tong-quan")]
        public async Task<IActionResult> GetThongKeTongQuan(
            [FromQuery] string filter = "thang",
            [FromQuery] string ngay = null,
            [FromQuery] string tuan = null,
            [FromQuery] string thang = null,
            [FromQuery] string tuNgay = null,
            [FromQuery] string denNgay = null)
        {
            var result = await _thongKeService.GetThongKeTongQuanAsync(filter, ngay, tuan, thang, tuNgay, denNgay);
            return Ok(result);
        }

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel([FromQuery] string filter = "thang")
        {
            var fileBytes = await _thongKeService.ExportExcelAsync(filter);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ThongKe.xlsx");
        }

        [HttpGet("hoa-don-san-pham-chart")]
        public async Task<IActionResult> GetHoaDonSanPhamChart(
            [FromQuery] string filter = "thang",
            [FromQuery] string ngay = null,
            [FromQuery] string tuan = null,
            [FromQuery] string thang = null,
            [FromQuery] string tuNgay = null,
            [FromQuery] string denNgay = null)
        {
            var result = await _thongKeService.GetThongKeHoaDonSanPhamChartAsync(filter, ngay, tuan, thang, tuNgay, denNgay);
            return Ok(result);
        }

        [HttpGet("top-san-pham-ban-chay")]
        public async Task<IActionResult> GetTopSanPhamBanChay(
            [FromQuery] int top = 5,
            [FromQuery] string filter = "thang",
            [FromQuery] string ngay = null,
            [FromQuery] string tuan = null,
            [FromQuery] string thang = null,
            [FromQuery] string tuNgay = null,
            [FromQuery] string denNgay = null)
        {
            var result = await _thongKeService.GetTopSanPhamBanChayAsync(top, filter, ngay, tuan, thang, tuNgay, denNgay);
            return Ok(result);
        }

        [HttpGet("trang-thai-don-hang")]
        public async Task<IActionResult> GetTrangThaiDonHang(
            [FromQuery] string filter = "thang",
            [FromQuery] string ngay = null,
            [FromQuery] string tuan = null,
            [FromQuery] string thang = null,
            [FromQuery] string tuNgay = null,
            [FromQuery] string denNgay = null)
        {
            var result = await _thongKeService.GetTrangThaiDonHangAsync(filter, ngay, tuan, thang, tuNgay, denNgay);
            return Ok(result);
        }

        [HttpGet("san-pham-sap-het-hang")]
        public async Task<IActionResult> GetSanPhamSapHetHang([FromQuery] int soLuongCanhBao = 5)
        {
            var result = await _thongKeService.GetSanPhamSapHetHangAsync(soLuongCanhBao);
            return Ok(result);
        }

        [HttpGet("toc-do-tang-truong")]
        public async Task<IActionResult> GetTocDoTangTruong()
        {
            var result = await _thongKeService.GetTocDoTangTruongAsync();
            return Ok(result);
        }

        [HttpGet("chi-tiet-ban-chay")]
        public async Task<IActionResult> GetSanPhamChiTietBanChayThongKe(
            Guid sanPhamId,
            [FromQuery] string filter = "thang",
            [FromQuery] string ngay = null,
            [FromQuery] string tuan = null,
            [FromQuery] string thang = null,
            [FromQuery] string tuNgay = null,
            [FromQuery] string denNgay = null)
        {
            var result = await _thongKeService.GetSanPhamChiTietBanChayThongKe(sanPhamId, filter, ngay, tuan, thang, tuNgay, denNgay);
            return Ok(result);
        }

        [HttpGet("chi-tiet-het-hang")]
        public async Task<IActionResult> GetSanPhamChiTietHetHangThongKe(Guid sanPhamId, int soLuongCanhBao = 5)
        {
            var result = await _thongKeService.GetSanPhamChiTietHetHangThongKe(sanPhamId, soLuongCanhBao);
            return Ok(result);
        }

    }
}
