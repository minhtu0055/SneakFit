using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.ThongKe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace SneakFit.Application.Catalog.ThongKe
{
    public class ThongKeService : IThongKeService
    {
        private readonly SneakFitDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ThongKeService(SneakFitDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ThongKeTongQuanViewModel> GetThongKeTongQuanAsync(
            string filter,
            string ngay = null,
            string tuan = null,
            string thang = null,
            string tuNgay = null,
            string denNgay = null
        )
        {
            // Xử lý filter ở đây (ngày, 7ngay, thang, nam, tuychinh)
            var today = DateTime.Today;
            DateTime fromDate, toDate;

            switch (filter?.ToLower())
            {
                case "homnay":
                    fromDate = today;
                    toDate = today.AddDays(1);
                    break;
                case "homqua":
                    fromDate = today.AddDays(-1);
                    toDate = today;
                    today = today.AddDays(-1); // Để các so sánh .Date == today đúng cho hôm qua
                    break;
                case "7ngay":
                    fromDate = today.AddDays(-6);
                    toDate = today.AddDays(1);
                    break;
                case "thang":
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = fromDate.AddMonths(1);
                    break;
                case "nam":
                    fromDate = new DateTime(today.Year, 1, 1);
                    toDate = fromDate.AddYears(1);
                    break;
                case "tuychinh":
                    // Sẽ xử lý ở bước tiếp theo
                    fromDate = today;
                    toDate = today.AddDays(1);
                    break;
                default:
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = fromDate.AddMonths(1);
                    break;
            }

            if (filter?.ToLower() == "tuychinh")
            {
                if (!string.IsNullOrEmpty(ngay))
                {
                    fromDate = DateTime.Parse(ngay);
                    toDate = fromDate.AddDays(1);
                }
                else if (!string.IsNullOrEmpty(tuan))
                {
                    // tuan dạng "yyyy-Www"
                    var parts = tuan.Split("-W");
                    int year = int.Parse(parts[0]);
                    int week = int.Parse(parts[1]);
                    fromDate = FirstDateOfWeekISO8601(year, week);
                    toDate = fromDate.AddDays(7);
                }
                else if (!string.IsNullOrEmpty(thang))
                {
                    var parts = thang.Split('-');
                    int year = int.Parse(parts[0]);
                    int month = int.Parse(parts[1]);
                    fromDate = new DateTime(year, month, 1);
                    toDate = fromDate.AddMonths(1);
                }
                else if (!string.IsNullOrEmpty(tuNgay) && !string.IsNullOrEmpty(denNgay))
                {
                    fromDate = DateTime.Parse(tuNgay);
                    toDate = DateTime.Parse(denNgay).AddDays(1);
                }
                else
                {
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = fromDate.AddMonths(1);
                }
            }

            var don = await _context.HoaDon
                .Where(x => x.NgayTao >= fromDate && x.NgayTao < toDate)
                .Where(x =>
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
                .ToListAsync();

            var donHomNay = don
                .Where(x =>
                    x.NgayTao.Date == today &&
                    (
                        (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                        ||
                        (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        ||
                        (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    )
                )
                .ToList();

            var soLuongSanPhamBan = await _context.HoaDonChiTiet
                .Where(x => x.HoaDon.NgayTao >= fromDate && x.HoaDon.NgayTao < toDate
                    && (
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.HoaDon.TrangThai != TrangThaiHoaDon.DaHuy && x.HoaDon.TrangThai != TrangThaiHoaDon.TraHang)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    )
                )
                .SumAsync(x => x.SoLuong);

            return new ThongKeTongQuanViewModel
            {
                SoDonThangNay = don.Count,
                DoanhSoThangNay = don.Sum(x => x.TongTien - x.PhiVanChuyen),
                SoDonHomNay = donHomNay.Count,
                DoanhSoHomNay = donHomNay.Sum(x => x.TongTien - x.PhiVanChuyen),
                SoLuongSanPhamBanThangNay = soLuongSanPhamBan
            };
        }

        public async Task<byte[]> ExportExcelAsync(string filter)
        {
            // TODO: Viết code xuất Excel ở đây, ví dụ trả về file rỗng để build không lỗi:
            return new byte[0];
        }

        public async Task<ThongKeHoaDonSanPhamChartViewModel> GetThongKeHoaDonSanPhamChartAsync(
            string filter,
            string ngay = null,
            string tuan = null,
            string thang = null,
            string tuNgay = null,
            string denNgay = null
        )
        {
            var today = DateTime.Today;
            DateTime fromDate, toDate;
            int days = 30;

            switch (filter?.ToLower())
            {
                case "homnay":
                    fromDate = today;
                    toDate = today.AddDays(1);
                    days = 1;
                    break;
                case "homqua":
                    fromDate = today.AddDays(-1);
                    toDate = today;
                    days = 1;
                    break;
                case "7ngay":
                    fromDate = today.AddDays(-6);
                    toDate = today.AddDays(1);
                    days = 7;
                    break;
                case "thang":
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = fromDate.AddMonths(1);
                    days = DateTime.DaysInMonth(today.Year, today.Month);
                    break;
                case "nam":
                    fromDate = new DateTime(today.Year, 1, 1);
                    toDate = fromDate.AddYears(1);
                    days = 12;
                    break;
                case "tuychinh":
                    if (!string.IsNullOrEmpty(ngay))
                    {
                        fromDate = DateTime.Parse(ngay);
                        toDate = fromDate.AddDays(1);
                        days = 1;
                    }
                    else if (!string.IsNullOrEmpty(tuan))
                    {
                        var parts = tuan.Split("-W");
                        int year = int.Parse(parts[0]);
                        int week = int.Parse(parts[1]);
                        fromDate = FirstDateOfWeekISO8601(year, week);
                        toDate = fromDate.AddDays(7);
                        days = 7;
                    }
                    else if (!string.IsNullOrEmpty(thang))
                    {
                        var parts = thang.Split('-');
                        int year = int.Parse(parts[0]);
                        int month = int.Parse(parts[1]);
                        fromDate = new DateTime(year, month, 1);
                        toDate = fromDate.AddMonths(1);
                        days = DateTime.DaysInMonth(year, month);
                    }
                    else if (!string.IsNullOrEmpty(tuNgay) && !string.IsNullOrEmpty(denNgay))
                    {
                        fromDate = DateTime.Parse(tuNgay);
                        toDate = DateTime.Parse(denNgay).AddDays(1);
                        days = (toDate - fromDate).Days;
                    }
                    else
                    {
                        fromDate = new DateTime(today.Year, today.Month, 1);
                        toDate = fromDate.AddMonths(1);
                        days = DateTime.DaysInMonth(today.Year, today.Month);
                    }
                    break;
                default:
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = fromDate.AddMonths(1);
                    days = DateTime.DaysInMonth(today.Year, today.Month);
                    break;
            }

            var labels = new List<string>();
            var soLuongHoaDon = new List<int>();
            var soLuongSanPham = new List<int>();

            if (filter == "nam")
            {
                // Theo tháng trong năm
                for (int i = 1; i <= 12; i++)
                {
                    var start = new DateTime(today.Year, i, 1);
                    var end = start.AddMonths(1);

                    var hoaDon = await _context.HoaDon
                        .Where(x => x.NgayTao >= start && x.NgayTao < end)
                        .Where(x =>
                            (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                            ||
                            (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            ||
                            (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        )
                        .ToListAsync();

                    var sanPham = await _context.HoaDonChiTiet
                        .Where(x => x.HoaDon.NgayTao >= start && x.HoaDon.NgayTao < end
                            && (
                                (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.HoaDon.TrangThai != TrangThaiHoaDon.DaHuy && x.HoaDon.TrangThai != TrangThaiHoaDon.TraHang)
                                ||
                                (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                                ||
                                (x.HoaDon.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            )
                        )
                        .SumAsync(x => x.SoLuong);

                    labels.Add($"{i}/{today.Year}");
                    soLuongHoaDon.Add(hoaDon.Count);
                    soLuongSanPham.Add(sanPham);
                }
            }
            else
            {
                // Theo ngày
                for (int i = 0; i < days; i++)
                {
                    var date = fromDate.AddDays(i);
                    var hoaDon = await _context.HoaDon
                        .Where(x => x.NgayTao.Date == date)
                        .Where(x =>
                            (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                            ||
                            (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            ||
                            (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        )
                        .ToListAsync();

                    var sanPham = await _context.HoaDonChiTiet
                        .Where(x => x.HoaDon.NgayTao.Date == date
                            && (
                                (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.HoaDon.TrangThai != TrangThaiHoaDon.DaHuy && x.HoaDon.TrangThai != TrangThaiHoaDon.TraHang)
                                ||
                                (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                                ||
                                (x.HoaDon.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            )
                        )
                        .SumAsync(x => x.SoLuong);

                    labels.Add(date.ToString("dd/MM/yyyy"));
                    soLuongHoaDon.Add(hoaDon.Count);
                    soLuongSanPham.Add(sanPham);
                }
            }

            return new ThongKeHoaDonSanPhamChartViewModel
            {
                Labels = labels,
                SoLuongHoaDon = soLuongHoaDon,
                SoLuongSanPham = soLuongSanPham
            };
        }

        public async Task<List<TopSanPhamBanChayViewModel>> GetTopSanPhamBanChayAsync(
            int top, string filter, string ngay = null, string tuan = null, string thang = null, string tuNgay = null, string denNgay = null)
        {
            var today = DateTime.Today;
            DateTime fromDate, toDate;

            // Copy logic xác định fromDate, toDate từ GetThongKeTongQuanAsync
            switch (filter?.ToLower())
            {
                case "homnay":
                    fromDate = today;
                    toDate = today.AddDays(1);
                    break;
                case "homqua":
                    fromDate = today.AddDays(-1);
                    toDate = today;
                    break;
                case "7ngay":
                    fromDate = today.AddDays(-6);
                    toDate = today.AddDays(1);
                    break;
                case "thang":
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = fromDate.AddMonths(1);
                    break;
                case "nam":
                    fromDate = new DateTime(today.Year, 1, 1);
                    toDate = fromDate.AddYears(1);
                    break;
                case "tuychinh":
                    if (!string.IsNullOrEmpty(ngay))
                    {
                        fromDate = DateTime.Parse(ngay);
                        toDate = fromDate.AddDays(1);
                    }
                    else if (!string.IsNullOrEmpty(tuan))
                    {
                        var parts = tuan.Split("-W");
                        int year = int.Parse(parts[0]);
                        int week = int.Parse(parts[1]);
                        fromDate = FirstDateOfWeekISO8601(year, week);
                        toDate = fromDate.AddDays(7);
                    }
                    else if (!string.IsNullOrEmpty(thang))
                    {
                        var parts = thang.Split('-');
                        int year = int.Parse(parts[0]);
                        int month = int.Parse(parts[1]);
                        fromDate = new DateTime(year, month, 1);
                        toDate = fromDate.AddMonths(1);
                    }
                    else if (!string.IsNullOrEmpty(tuNgay) && !string.IsNullOrEmpty(denNgay))
                    {
                        fromDate = DateTime.Parse(tuNgay);
                        toDate = DateTime.Parse(denNgay).AddDays(1);
                    }
                    else
                    {
                        fromDate = new DateTime(today.Year, today.Month, 1);
                        toDate = fromDate.AddMonths(1);
                    }
                    break;
                default:
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = fromDate.AddMonths(1);
                    break;
            }

            var query = from hdct in _context.HoaDonChiTiet
                        join spct in _context.SanPhamChiTiet on hdct.SanPhamChiTietId equals spct.ID
                        join sp in _context.SanPham on spct.SanPhamId equals sp.Id
                        join dm in _context.DanhMuc on sp.DanhMucId equals dm.Id
                        where hdct.HoaDon.NgayTao >= fromDate && hdct.HoaDon.NgayTao < toDate
                            && (
                                (hdct.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (hdct.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || hdct.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && hdct.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && hdct.HoaDon.TrangThai != TrangThaiHoaDon.DaHuy && hdct.HoaDon.TrangThai != TrangThaiHoaDon.TraHang)
                                ||
                                (hdct.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (hdct.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || hdct.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.COD) && hdct.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && hdct.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                                ||
                                (hdct.HoaDon.LoaiHoaDon == LoaiHoaDon.TaiQuay && hdct.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && hdct.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            )
                        group new { hdct, spct, sp, dm } by new { sp.Id, sp.TenSanPham, sp.Mota, DanhMuc = dm.TenDanhMuc } into g
                        orderby g.Sum(x => x.hdct.SoLuong) descending
                        select new TopSanPhamBanChayViewModel
                        {
                            SanPhamId = g.Key.Id,
                            TenSanPham = g.Key.TenSanPham,
                            MoTa = g.Key.Mota,
                            DanhMuc = g.Key.DanhMuc,
                            SoLuongDaBan = g.Sum(x => x.hdct.SoLuong),
                            SoLuongSanPhamChiTiet = g.Select(x => x.spct.ID).Distinct().Count(),
                            STT = 0
                        };

            var result = await query
            .Where(x => x.SoLuongDaBan > 5) // Lọc sản phẩm bán > 5
            .ToListAsync();
            for (int i = 0; i < result.Count; i++)
            {
                result[i].STT = i + 1;
            }
            return result;
        }

        public async Task<List<TrangThaiDonHangViewModel>> GetTrangThaiDonHangAsync(
            string filter, string ngay = null, string tuan = null, string thang = null, string tuNgay = null, string denNgay = null)
        {
            var today = DateTime.Today;
            DateTime fromDate, toDate;

            // Xác định khoảng thời gian theo filter
            switch (filter?.ToLower())
            {
                case "homnay":
                    fromDate = today;
                    toDate = today.AddDays(1);
                    break;
                case "homqua":
                    fromDate = today.AddDays(-1);
                    toDate = today;
                    break;
                case "7ngay":
                    fromDate = today.AddDays(-6);
                    toDate = today.AddDays(1);
                    break;
                case "thang":
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = fromDate.AddMonths(1);
                    break;
                case "nam":
                    fromDate = new DateTime(today.Year, 1, 1);
                    toDate = fromDate.AddYears(1);
                    break;
                case "tuychinh":
                    if (!string.IsNullOrEmpty(ngay))
                    {
                        fromDate = DateTime.Parse(ngay);
                        toDate = fromDate.AddDays(1);
                    }
                    else if (!string.IsNullOrEmpty(tuan))
                    {
                        var parts = tuan.Split("-W");
                        int year = int.Parse(parts[0]);
                        int week = int.Parse(parts[1]);
                        fromDate = FirstDateOfWeekISO8601(year, week);
                        toDate = fromDate.AddDays(7);
                    }
                    else if (!string.IsNullOrEmpty(thang))
                    {
                        var parts = thang.Split('-');
                        int year = int.Parse(parts[0]);
                        int month = int.Parse(parts[1]);
                        fromDate = new DateTime(year, month, 1);
                        toDate = fromDate.AddMonths(1);
                    }
                    else if (!string.IsNullOrEmpty(tuNgay) && !string.IsNullOrEmpty(denNgay))
                    {
                        fromDate = DateTime.Parse(tuNgay);
                        toDate = DateTime.Parse(denNgay).AddDays(1);
                    }
                    else
                    {
                        fromDate = new DateTime(today.Year, today.Month, 1);
                        toDate = fromDate.AddMonths(1);
                    }
                    break;
                default:
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = fromDate.AddMonths(1);
                    break;
            }

            // Lọc hóa đơn theo khoảng thời gian
            var donTrongKhoang = await _context.HoaDon
                .Where(x => x.NgayTao >= fromDate && x.NgayTao < toDate)
                .ToListAsync();

            var tong = donTrongKhoang.Count;

            // Lấy tất cả trạng thái từ enum
            var allTrangThai = Enum.GetValues(typeof(TrangThaiHoaDon)).Cast<TrangThaiHoaDon>();

            var result = allTrangThai.Select(tt =>
            {
                var soLuong = donTrongKhoang.Count(x => x.TrangThai == tt);
                return new TrangThaiDonHangViewModel
                {
                    TrangThai = tt.ToString(),
                    SoLuong = soLuong,
                    TiLe = tong > 0 ? Math.Round(soLuong * 100.0 / tong, 2) : 0
                };
            }).ToList();

            return result;
        }

        public async Task<List<SanPhamSapHetHangViewModel>> GetSanPhamSapHetHangAsync(int soLuongCanhBao = 5)
        {
            var query = from sp in _context.SanPham
                        join dm in _context.DanhMuc on sp.DanhMucId equals dm.Id
                        where _context.SanPhamChiTiet.Any(spct => spct.SanPhamId == sp.Id && spct.SoLuong <= soLuongCanhBao && spct.SoLuong > 0)
                        select new SanPhamSapHetHangViewModel
                        {
                            SanPhamId = sp.Id,
                            TenSanPham = sp.TenSanPham,
                            MoTa = sp.Mota,
                            DanhMuc = dm.TenDanhMuc,
                            SoLuongSanPhamChiTiet = _context.SanPhamChiTiet.Count(spct => spct.SanPhamId == sp.Id && spct.SoLuong > 0 && spct.SoLuong <= 5),
                            STT = 0 // sẽ cập nhật sau
                        };

            var result = await query.ToListAsync();
            for (int i = 0; i < result.Count; i++)
            {
                result[i].STT = i + 1;
            }
            return result;
        }



        public async Task<List<TocDoTangTruongViewModel>> GetTocDoTangTruongAsync()
        {
            var today = DateTime.Today;
            var endOfToday = today.AddDays(1);
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var firstDayOfYear = new DateTime(today.Year, 1, 1);

            // Doanh thu ngày
            var doanhThuNgay = await _context.HoaDon
            .Where(x => x.NgayTao.Date == today)
            .Where(x =>
                (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                ||
                (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                ||
                (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
            )
            .SumAsync(x => x.TongTien - x.PhiVanChuyen);

            var doanhThuNgayTruoc = await _context.HoaDon
                .Where(x => x.NgayTao.Date == today.AddDays(-1))
                .Where(x =>
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
                .SumAsync(x => x.TongTien - x.PhiVanChuyen);

            // Doanh thu tuần
            // Xác định ngày đầu tuần (Thứ Hai) và cuối tuần (Chủ nhật, hết ngày)
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (today.DayOfWeek == DayOfWeek.Sunday ? -6 : 1)); // Đầu tuần (Thứ Hai)
            var endOfWeek = startOfWeek.AddDays(7); // Đầu tuần sau (không bao gồm)
            var startOfLastWeek = startOfWeek.AddDays(-7); // Đầu tuần trước
            var endOfLastWeek = startOfWeek; // Đầu tuần này (không bao gồm)

            var doanhThuTuan = await _context.HoaDon
                .Where(x => x.NgayTao >= startOfWeek && x.NgayTao < endOfWeek)
                .Where(x =>
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
                .SumAsync(x => x.TongTien - x.PhiVanChuyen);

            var doanhThuTuanTruoc = await _context.HoaDon
                 .Where(x => x.NgayTao >= startOfLastWeek && x.NgayTao < endOfLastWeek)
                .Where(x => x.NgayTao >= startOfLastWeek && x.NgayTao < startOfWeek)
                .Where(x =>
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
                .SumAsync(x => x.TongTien - x.PhiVanChuyen);

            // Doanh thu tháng
            var doanhThuThang = await _context.HoaDon
                .Where(x => x.NgayTao >= firstDayOfMonth && x.NgayTao < endOfToday)
                .Where(x =>
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
                .SumAsync(x => x.TongTien - x.PhiVanChuyen);

            var doanhThuThangTruoc = await _context.HoaDon
                .Where(x => x.NgayTao >= firstDayOfMonth.AddMonths(-1) && x.NgayTao < firstDayOfMonth)
                .Where(x =>
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
                .SumAsync(x => x.TongTien - x.PhiVanChuyen);

            // Doanh thu năm
            var doanhThuNam = await _context.HoaDon
                .Where(x => x.NgayTao >= firstDayOfYear && x.NgayTao < endOfToday)
                .Where(x =>
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
                .SumAsync(x => x.TongTien - x.PhiVanChuyen);

            var doanhThuNamTruoc = await _context.HoaDon
                .Where(x => x.NgayTao >= firstDayOfYear.AddYears(-1) && x.NgayTao < firstDayOfYear)
                .Where(x =>
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
                .SumAsync(x => x.TongTien - x.PhiVanChuyen);


            // Sản phẩm bán
            // Sản phẩm ngày
            var sanPhamNgay = await _context.HoaDonChiTiet
                .Where(x => x.HoaDon.NgayTao.Date == today
                    && (
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.HoaDon.TrangThai != TrangThaiHoaDon.DaHuy && x.HoaDon.TrangThai != TrangThaiHoaDon.TraHang)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    )
                )
                .SumAsync(x => x.SoLuong);

            var sanPhamNgayTruoc = await _context.HoaDonChiTiet
                .Where(x => x.HoaDon.NgayTao.Date == today.AddDays(-1)
                    && (
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.HoaDon.TrangThai != TrangThaiHoaDon.DaHuy && x.HoaDon.TrangThai != TrangThaiHoaDon.TraHang)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    )
                )
                .SumAsync(x => x.SoLuong);

            // Sản phẩm tuần
            var sanPhamTuan = await _context.HoaDonChiTiet
                 .Where(x => x.HoaDon.NgayTao >= startOfWeek && x.HoaDon.NgayTao < endOfWeek
                     && (
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.HoaDon.TrangThai != TrangThaiHoaDon.DaHuy && x.HoaDon.TrangThai != TrangThaiHoaDon.TraHang)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    )
                )
                 .SumAsync(x => x.SoLuong);

            var sanPhamTuanTruoc = await _context.HoaDonChiTiet
                .Where(x => x.HoaDon.NgayTao >= startOfLastWeek && x.HoaDon.NgayTao < endOfLastWeek
                    && (
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.HoaDon.TrangThai != TrangThaiHoaDon.DaHuy && x.HoaDon.TrangThai != TrangThaiHoaDon.TraHang)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    )
                )
                .SumAsync(x => x.SoLuong);

            // Sản phẩm tháng
            var sanPhamThang = await _context.HoaDonChiTiet
                .Where(x => x.HoaDon.NgayTao >= firstDayOfMonth && x.HoaDon.NgayTao < endOfToday
                    && (
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.HoaDon.TrangThai != TrangThaiHoaDon.DaHuy && x.HoaDon.TrangThai != TrangThaiHoaDon.TraHang)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    )
                )
                .SumAsync(x => x.SoLuong);

            var sanPhamThangTruoc = await _context.HoaDonChiTiet
                .Where(x => x.HoaDon.NgayTao >= firstDayOfMonth.AddMonths(-1) && x.HoaDon.NgayTao < firstDayOfMonth
                    && (
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.HoaDon.TrangThai != TrangThaiHoaDon.DaHuy && x.HoaDon.TrangThai != TrangThaiHoaDon.TraHang)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    )
                )
                .SumAsync(x => x.SoLuong);

            // Sản phẩm năm
            var sanPhamNam = await _context.HoaDonChiTiet
                 .Where(x => x.HoaDon.NgayTao >= firstDayOfYear && x.HoaDon.NgayTao < endOfToday
                     && (
                            (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.HoaDon.TrangThai != TrangThaiHoaDon.DaHuy && x.HoaDon.TrangThai != TrangThaiHoaDon.TraHang)
                            ||
                            (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            ||
                            (x.HoaDon.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        )
                    )
                 .SumAsync(x => x.SoLuong);

            var sanPhamNamTruoc = await _context.HoaDonChiTiet
                .Where(x => x.HoaDon.NgayTao >= firstDayOfYear.AddYears(-1) && x.HoaDon.NgayTao < firstDayOfYear
                    && (
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.HoaDon.TrangThai != TrangThaiHoaDon.DaHuy && x.HoaDon.TrangThai != TrangThaiHoaDon.TraHang)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        ||
                        (x.HoaDon.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && x.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    )
                )
                .SumAsync(x => x.SoLuong);

            // Đơn hàng
            var hoaDonNgay = await _context.HoaDon.CountAsync(x =>
                x.NgayTao.Date == today &&
                (
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
            );

            var hoaDonNgayTruoc = await _context.HoaDon.CountAsync(x =>
                x.NgayTao.Date == today.AddDays(-1) &&
                (
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
            );

            var hoaDonTuan = await _context.HoaDon.CountAsync(x =>
                x.NgayTao >= startOfWeek && x.NgayTao < endOfWeek &&
                (
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
            );

            var hoaDonTuanTruoc = await _context.HoaDon.CountAsync(x =>
                x.NgayTao >= startOfLastWeek && x.NgayTao < endOfLastWeek &&
                (
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
            );

            // Hóa đơn tháng
            var hoaDonThang = await _context.HoaDon.CountAsync(x =>
                x.NgayTao >= firstDayOfMonth && x.NgayTao < endOfToday &&
                (
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
            );

            var hoaDonThangTruoc = await _context.HoaDon.CountAsync(x =>
                x.NgayTao >= firstDayOfMonth.AddMonths(-1) && x.NgayTao < firstDayOfMonth &&
                (
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
            );

            // Hóa đơn năm
            var hoaDonNam = await _context.HoaDon.CountAsync(x =>
                x.NgayTao >= firstDayOfYear && x.NgayTao < endOfToday &&
                (
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
            );

            var hoaDonNamTruoc = await _context.HoaDon.CountAsync(x =>
                x.NgayTao >= firstDayOfYear.AddYears(-1) && x.NgayTao < firstDayOfYear &&
                (
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || x.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai != TrangThaiHoaDon.DaHuy && x.TrangThai != TrangThaiHoaDon.TraHang)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.Online && (x.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || x.PhuongThucThanhToan == PhuongThucThanhToan.COD) && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    ||
                    (x.LoaiHoaDon == LoaiHoaDon.TaiQuay && x.TrangThai == TrangThaiHoaDon.ThanhCong && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                )
            );

            // Helper tính phần trăm tăng trưởng
            double CalcPercent(decimal now, decimal prev)
            {
                if (prev == 0)
                    return now > 0 ? 100 : 0;
                var percent = Math.Round((double)(now - prev) / (double)Math.Abs(prev) * 100, 2);
                return percent > 100 ? 100 : percent;
            }

            var result = new List<TocDoTangTruongViewModel>
            {
                new TocDoTangTruongViewModel { TenChiSo = "Doanh thu ngày", GiaTriHienTai = doanhThuNgay, GiaTriTruocDo = doanhThuNgayTruoc, PhanTramTangTruong = CalcPercent(doanhThuNgay, doanhThuNgayTruoc), DonVi = "VND" },
                new TocDoTangTruongViewModel { TenChiSo = "Doanh thu tuần", GiaTriHienTai = doanhThuTuan, GiaTriTruocDo = doanhThuTuanTruoc, PhanTramTangTruong = CalcPercent(doanhThuTuan, doanhThuTuanTruoc), DonVi = "VND" },
                new TocDoTangTruongViewModel { TenChiSo = "Doanh thu tháng", GiaTriHienTai = doanhThuThang, GiaTriTruocDo = doanhThuThangTruoc, PhanTramTangTruong = CalcPercent(doanhThuThang, doanhThuThangTruoc), DonVi = "VND" },
                new TocDoTangTruongViewModel { TenChiSo = "Doanh thu năm", GiaTriHienTai = doanhThuNam, GiaTriTruocDo = doanhThuNamTruoc, PhanTramTangTruong = CalcPercent(doanhThuNam, doanhThuNamTruoc), DonVi = "VND" },

                new TocDoTangTruongViewModel { TenChiSo = "Sản phẩm ngày", GiaTriHienTai = sanPhamNgay, GiaTriTruocDo = sanPhamNgayTruoc, PhanTramTangTruong = CalcPercent(sanPhamNgay, sanPhamNgayTruoc), DonVi = "Sản phẩm" },
                new TocDoTangTruongViewModel { TenChiSo = "Sản phẩm tuần", GiaTriHienTai = sanPhamTuan, GiaTriTruocDo = sanPhamTuanTruoc, PhanTramTangTruong = CalcPercent(sanPhamTuan, sanPhamTuanTruoc), DonVi = "Sản phẩm" },
                new TocDoTangTruongViewModel { TenChiSo = "Sản phẩm tháng", GiaTriHienTai = sanPhamThang, GiaTriTruocDo = sanPhamThangTruoc, PhanTramTangTruong = CalcPercent(sanPhamThang, sanPhamThangTruoc), DonVi = "Sản phẩm" },
                new TocDoTangTruongViewModel { TenChiSo = "Sản phẩm năm", GiaTriHienTai = sanPhamNam, GiaTriTruocDo = sanPhamNamTruoc, PhanTramTangTruong = CalcPercent(sanPhamNam, sanPhamNamTruoc), DonVi = "Sản phẩm" },

                new TocDoTangTruongViewModel { TenChiSo = "Hóa đơn ngày", GiaTriHienTai = hoaDonNgay, GiaTriTruocDo = hoaDonNgayTruoc, PhanTramTangTruong = CalcPercent(hoaDonNgay, hoaDonNgayTruoc), DonVi = "Hóa đơn" },
                new TocDoTangTruongViewModel { TenChiSo = "Hóa đơn tuần", GiaTriHienTai = hoaDonTuan, GiaTriTruocDo = hoaDonTuanTruoc, PhanTramTangTruong = CalcPercent(hoaDonTuan, hoaDonTuanTruoc), DonVi = "Hóa đơn" },
                new TocDoTangTruongViewModel { TenChiSo = "Hóa đơn tháng", GiaTriHienTai = hoaDonThang, GiaTriTruocDo = hoaDonThangTruoc, PhanTramTangTruong = CalcPercent(hoaDonThang, hoaDonThangTruoc), DonVi = "Hóa đơn" },
                new TocDoTangTruongViewModel { TenChiSo = "Hóa đơn năm", GiaTriHienTai = hoaDonNam, GiaTriTruocDo = hoaDonNamTruoc, PhanTramTangTruong = CalcPercent(hoaDonNam, hoaDonNamTruoc), DonVi = "Hóa đơn" },
            };

            return result;
        }

        public async Task<List<SanPhamChiTietThongKeViewModel>> GetSanPhamChiTietBanChayThongKe(
     Guid sanPhamId, string filter, string ngay = null, string tuan = null, string thang = null, string tuNgay = null, string denNgay = null)
        {
            var today = DateTime.Today;
            DateTime fromDate, toDate;

            // Copy logic xác định fromDate, toDate từ các hàm filter khác
            switch (filter?.ToLower())
            {
                case "homnay":
                    fromDate = today;
                    toDate = today.AddDays(1);
                    break;
                case "homqua":
                    fromDate = today.AddDays(-1);
                    toDate = today;
                    break;
                case "7ngay":
                    fromDate = today.AddDays(-6);
                    toDate = today.AddDays(1);
                    break;
                case "thang":
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = fromDate.AddMonths(1);
                    break;
                case "nam":
                    fromDate = new DateTime(today.Year, 1, 1);
                    toDate = fromDate.AddYears(1);
                    break;
                case "tuychinh":
                    if (!string.IsNullOrEmpty(ngay))
                    {
                        fromDate = DateTime.Parse(ngay);
                        toDate = fromDate.AddDays(1);
                    }
                    else if (!string.IsNullOrEmpty(tuan))
                    {
                        var parts = tuan.Split("-W");
                        int year = int.Parse(parts[0]);
                        int week = int.Parse(parts[1]);
                        fromDate = FirstDateOfWeekISO8601(year, week);
                        toDate = fromDate.AddDays(7);
                    }
                    else if (!string.IsNullOrEmpty(thang))
                    {
                        var parts = thang.Split('-');
                        int year = int.Parse(parts[0]);
                        int month = int.Parse(parts[1]);
                        fromDate = new DateTime(year, month, 1);
                        toDate = fromDate.AddMonths(1);
                    }
                    else if (!string.IsNullOrEmpty(tuNgay) && !string.IsNullOrEmpty(denNgay))
                    {
                        fromDate = DateTime.Parse(tuNgay);
                        toDate = DateTime.Parse(denNgay).AddDays(1);
                    }
                    else
                    {
                        fromDate = new DateTime(today.Year, today.Month, 1);
                        toDate = fromDate.AddMonths(1);
                    }
                    break;
                default:
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = fromDate.AddMonths(1);
                    break;
            }

            var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

            var query = from hdct in _context.HoaDonChiTiet
                        join spct in _context.SanPhamChiTiet on hdct.SanPhamChiTietId equals spct.ID
                        join sp in _context.SanPham on spct.SanPhamId equals sp.Id
                        join ms in _context.MauSac on spct.MauSacId equals ms.Id
                        join kt in _context.KichThuoc on spct.KichThuocId equals kt.Id
                        join cl in _context.ChatLieu on spct.ChatLieuId equals cl.Id
                        join dg in _context.DeGiay on spct.DeGiayId equals dg.Id
                        join th in _context.ThuongHieu on spct.ThuongHieuId equals th.Id
                        where spct.SanPhamId == sanPhamId
                        && hdct.HoaDon.NgayTao >= fromDate && hdct.HoaDon.NgayTao < toDate
                        && (
                            (hdct.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (hdct.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || hdct.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.MoMo) && hdct.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && hdct.HoaDon.TrangThai != TrangThaiHoaDon.DaHuy && hdct.HoaDon.TrangThai != TrangThaiHoaDon.TraHang)
                            ||
                            (hdct.HoaDon.LoaiHoaDon == LoaiHoaDon.Online && (hdct.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.TienMat || hdct.HoaDon.PhuongThucThanhToan == PhuongThucThanhToan.COD) && hdct.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && hdct.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            ||
                            (hdct.HoaDon.LoaiHoaDon == LoaiHoaDon.TaiQuay && hdct.HoaDon.TrangThai == TrangThaiHoaDon.ThanhCong && hdct.HoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        )
                        group new { hdct, spct, sp, ms, kt, cl, dg, th } by new
                        {
                            spct.ID,
                            sp.TenSanPham,
                            ms.TenMauSac,
                            TenKichThuoc = kt.MaKichThuoc.ToString(),
                            cl.TenChatLieu,
                            dg.TenDeGiay,
                            spct.Gia,
                            spct.SoLuong,
                            ThuongHieu = th.TenThuongHieu
                        } into g
                        select new SanPhamChiTietThongKeViewModel
                        {
                            STT = 0,
                            SanPhamChiTietId = g.Key.ID,
                            Anh = baseUrl + (
                                _context.HinhAnhSanPham
                                    .Where(h => h.SanPhamChiTietId == g.Key.ID)
                                    .Select(h => h.UrlHinhAnh)
                                    .FirstOrDefault() ?? "/images/no-image.png"
                            ),
                            TenSanPham = g.Key.TenSanPham,
                            MauSac = g.Key.TenMauSac,
                            KichThuoc = g.Key.TenKichThuoc,
                            ChatLieu = g.Key.TenChatLieu,
                            DeGiay = g.Key.TenDeGiay,
                            ThuongHieu = g.Key.ThuongHieu,
                            Gia = g.Key.Gia,
                            SoLuongConLai = g.Key.SoLuong,
                            SoLuongDaBan = g.Sum(x => x.hdct.SoLuong)
                        };

            var result = await query.ToListAsync();
            for (int i = 0; i < result.Count; i++)
                result[i].STT = i + 1;
            return result;
        }


        public async Task<List<SanPhamChiTietThongKeViewModel>> GetSanPhamChiTietHetHangThongKe(Guid sanPhamId, int soLuongCanhBao = 5)
        {
            var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

            var query = from spct in _context.SanPhamChiTiet
                        join sp in _context.SanPham on spct.SanPhamId equals sp.Id
                        join ms in _context.MauSac on spct.MauSacId equals ms.Id
                        join kt in _context.KichThuoc on spct.KichThuocId equals kt.Id
                        join cl in _context.ChatLieu on spct.ChatLieuId equals cl.Id
                        join dg in _context.DeGiay on spct.DeGiayId equals dg.Id
                        join th in _context.ThuongHieu on spct.ThuongHieuId equals th.Id
                        where spct.SanPhamId == sanPhamId
                              && spct.SoLuong <= soLuongCanhBao
                              && spct.SoLuong > 0
                        select new SanPhamChiTietThongKeViewModel
                        {
                            STT = 0,
                            SanPhamChiTietId = spct.ID,
                            Anh = baseUrl + (
                                _context.HinhAnhSanPham
                                    .Where(h => h.SanPhamChiTietId == spct.ID)
                                    .Select(h => h.UrlHinhAnh)
                                    .FirstOrDefault() ?? "/images/no-image.png"
                            ),
                            TenSanPham = sp.TenSanPham,
                            MauSac = ms.TenMauSac,
                            KichThuoc = kt.MaKichThuoc.ToString(), // Ép kiểu sang string
                            ChatLieu = cl.TenChatLieu,
                            DeGiay = dg.TenDeGiay,
                            ThuongHieu = th.TenThuongHieu,
                            Gia = spct.Gia,
                            SoLuongConLai = spct.SoLuong,
                            SoLuongDaBan = _context.HoaDonChiTiet.Where(hd => hd.SanPhamChiTietId == spct.ID).Sum(hd => (int?)hd.SoLuong) ?? 0
                        };

            var result = await query.ToListAsync();
            for (int i = 0; i < result.Count; i++)
                result[i].STT = i + 1;
            return result;
        }


        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar; // Fixed namespace issue
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }


    }
}
