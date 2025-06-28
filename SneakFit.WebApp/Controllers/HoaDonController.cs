using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Common;
using System.Globalization;

namespace SneakFit.Admin.Controllers
{
    public class HoaDonController : Controller
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
            return View(hoaDon);
        }   
    }
}
