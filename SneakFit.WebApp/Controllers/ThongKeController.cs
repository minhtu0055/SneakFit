using Microsoft.AspNetCore.Mvc;
using SneakFit.ViewModels.Catalog.ThongKe;

namespace SneakFit.Admin.Controllers
{
    public class ThongKeController : BaseController
    {
        private readonly HttpClient _httpClient;
        public ThongKeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(); // KHÔNG truyền tên
            _httpClient.BaseAddress = new Uri("https://localhost:7277/"); // Đặt BaseAddress ở đây
        }

        public async Task<IActionResult> Index(
            string filter = "thang",
            string ngay = null,
            string tuan = null,
            string thang = null,
            string tuNgay = null,
            string denNgay = null)
        {
            // Lấy dữ liệu theo filter
            var url = $"api/ThongKe/tong-quan?filter={filter}";
            if (!string.IsNullOrEmpty(ngay)) url += $"&ngay={ngay}";
            if (!string.IsNullOrEmpty(tuan)) url += $"&tuan={tuan}";
            if (!string.IsNullOrEmpty(thang)) url += $"&thang={thang}";
            if (!string.IsNullOrEmpty(tuNgay)) url += $"&tuNgay={tuNgay}";
            if (!string.IsNullOrEmpty(denNgay)) url += $"&denNgay={denNgay}";

            var response = await _httpClient.GetAsync(url);
            var thongKe = await response.Content.ReadFromJsonAsync<ThongKeTongQuanViewModel>();

            // Lấy dữ liệu hôm nay (luôn luôn)
            var responseHomNay = await _httpClient.GetAsync("api/ThongKe/tong-quan?filter=homnay");
            var thongKeHomNay = await responseHomNay.Content.ReadFromJsonAsync<ThongKeTongQuanViewModel>();

            ViewBag.Filter = filter;
            ViewBag.FilterTitle = GetFilterTitle(filter, ngay, tuan, thang, tuNgay, denNgay);
            ViewBag.ThongKeHomNay = thongKeHomNay;
            return View(thongKe);
        }

        public async Task<IActionResult> ExportExcel(string filter = "thang")
        {
            var response = await _httpClient.GetAsync($"api/ThongKe/export-excel?filter={filter}");
            var content = await response.Content.ReadAsByteArrayAsync();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ThongKe.xlsx");
        }
        public async Task<IActionResult> GetHoaDonSanPhamChart(
            string filter = "thang",
            string ngay = null,
            string tuan = null,
            string thang = null,
            string tuNgay = null,
            string denNgay = null)
        {
            var url = $"api/ThongKe/hoa-don-san-pham-chart?filter={filter}";
            if (!string.IsNullOrEmpty(ngay)) url += $"&ngay={ngay}";
            if (!string.IsNullOrEmpty(tuan)) url += $"&tuan={tuan}";
            if (!string.IsNullOrEmpty(thang)) url += $"&thang={thang}";
            if (!string.IsNullOrEmpty(tuNgay)) url += $"&tuNgay={tuNgay}";
            if (!string.IsNullOrEmpty(denNgay)) url += $"&denNgay={denNgay}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            var data = await response.Content.ReadFromJsonAsync<ThongKeHoaDonSanPhamChartViewModel>();
            return Json(data);
        }

        public async Task<IActionResult> GetTopSanPhamBanChay(
            int top = 1,
            string filter = "thang",
            string ngay = null,
            string tuan = null,
            string thang = null,
            string tuNgay = null,
            string denNgay = null)
        {
            var url = $"api/ThongKe/top-san-pham-ban-chay?top={top}&filter={filter}";
            if (!string.IsNullOrEmpty(ngay)) url += $"&ngay={ngay}";
            if (!string.IsNullOrEmpty(tuan)) url += $"&tuan={tuan}";
            if (!string.IsNullOrEmpty(thang)) url += $"&thang={thang}";
            if (!string.IsNullOrEmpty(tuNgay)) url += $"&tuNgay={tuNgay}";
            if (!string.IsNullOrEmpty(denNgay)) url += $"&denNgay={denNgay}";

            var response = await _httpClient.GetAsync(url);
            var data = await response.Content.ReadFromJsonAsync<List<TopSanPhamBanChayViewModel>>();
            return Json(data);
        }

        public async Task<IActionResult> GetTrangThaiDonHang(
            string filter = "thang",
            string ngay = null,
            string tuan = null,
            string thang = null,
            string tuNgay = null,
            string denNgay = null)
        {
            var url = $"api/ThongKe/trang-thai-don-hang?filter={filter}";
            if (!string.IsNullOrEmpty(ngay)) url += $"&ngay={ngay}";
            if (!string.IsNullOrEmpty(tuan)) url += $"&tuan={tuan}";
            if (!string.IsNullOrEmpty(thang)) url += $"&thang={thang}";
            if (!string.IsNullOrEmpty(tuNgay)) url += $"&tuNgay={tuNgay}";
            if (!string.IsNullOrEmpty(denNgay)) url += $"&denNgay={denNgay}";

            var response = await _httpClient.GetAsync(url);
            var data = await response.Content.ReadFromJsonAsync<List<TrangThaiDonHangViewModel>>();
            return Json(data);
        }

        public async Task<IActionResult> GetSanPhamSapHetHang(int soLuongCanhBao = 5)
        {
            var response = await _httpClient.GetAsync($"api/ThongKe/san-pham-sap-het-hang?soLuongCanhBao={soLuongCanhBao}");
            var data = await response.Content.ReadFromJsonAsync<List<SanPhamSapHetHangViewModel>>();
            return Json(data);
        }

        public async Task<IActionResult> GetTocDoTangTruong()
        {
            var response = await _httpClient.GetAsync("api/ThongKe/toc-do-tang-truong");
            var data = await response.Content.ReadFromJsonAsync<List<TocDoTangTruongViewModel>>();
            return Json(data);
        }

        // Proxy cho Top bán chạy
        [HttpGet]
        public async Task<IActionResult> GetSanPhamChiTietBanChayThongKe(
            Guid sanPhamId,
            string filter = "thang",
            string ngay = null,
            string tuan = null,
            string thang = null,
            string tuNgay = null,
            string denNgay = null)
        {
            var url = $"api/ThongKe/chi-tiet-ban-chay?sanPhamId={sanPhamId}&filter={filter}";
            if (!string.IsNullOrEmpty(ngay)) url += $"&ngay={ngay}";
            if (!string.IsNullOrEmpty(tuan)) url += $"&tuan={tuan}";
            if (!string.IsNullOrEmpty(thang)) url += $"&thang={thang}";
            if (!string.IsNullOrEmpty(tuNgay)) url += $"&tuNgay={tuNgay}";
            if (!string.IsNullOrEmpty(denNgay)) url += $"&denNgay={denNgay}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }

        // Proxy cho Sắp hết hàng
        [HttpGet]
        public async Task<IActionResult> GetSanPhamChiTietHetHangThongKe(Guid sanPhamId, int soLuongCanhBao = 5)
        {
            var response = await _httpClient.GetAsync($"api/ThongKe/chi-tiet-het-hang?sanPhamId={sanPhamId}&soLuongCanhBao={soLuongCanhBao}");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }

        private string GetFilterTitle(string filter, string ngay, string tuan, string thang, string tuNgay, string denNgay)
        {
            switch (filter)
            {
                case "homnay":
                    return "hôm nay";
                case "homqua":
                    return "hôm qua";
                case "7ngay":
                    return "7 ngày qua";
                case "thang":
                    return "tháng này";
                case "nam":
                    return "năm nay";
                case "tuychinh":
                    if (!string.IsNullOrEmpty(ngay))
                        return $"ngày {DateTime.Parse(ngay).ToString("dd/MM/yyyy")}";
                    if (!string.IsNullOrEmpty(tuan))
                        return $"tuần {tuan}";
                    if (!string.IsNullOrEmpty(thang))
                        return $"tháng {thang}";
                    if (!string.IsNullOrEmpty(tuNgay) && !string.IsNullOrEmpty(denNgay))
                        return $"từ {DateTime.Parse(tuNgay).ToString("dd/MM/yyyy")} đến {DateTime.Parse(denNgay).ToString("dd/MM/yyyy")}";
                    return "tuỳ chỉnh";
                default:
                    return "tháng này";
            }
        }

    }
}
