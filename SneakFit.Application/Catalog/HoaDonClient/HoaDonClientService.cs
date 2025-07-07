using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using SneakFit.ViewModels.Catalog.HoaDonClient;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.HoaDonClient
{
    public class HoaDonClientService : IHoaDonClientService
    {
        private readonly SneakFitDbContext _context;

        public HoaDonClientService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<HoaDonClientViewModel>> GetAllPaging(PhanTrangHoaDonClient request)
        {
            var query = _context.HoaDon
                .Include(h => h.HoaDonChiTiet)
                .ThenInclude(hdc => hdc.SanPhamChiTiet)
                .Include(h => h.User)
                .Include(h => h.Voucher)
                .AsQueryable();
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(h => h.HoTen.Contains(request.Keyword) || h.MaHoaDon.Contains(request.Keyword));
            }
            // Lọc theo trạng thái
            if (request.Trangthaihoadon.HasValue)
            {
                query = query.Where(x => x.TrangThai == request.Trangthaihoadon.Value);
            }
            // Lọc theo ngày tạo trong khoảng thời gian từ ngày đến ngày
            if (request.NgayBatDau.HasValue && request.NgayKetThuc.HasValue)
            {
                query = query.Where(h => h.NgayTao >= request.NgayBatDau.Value && h.NgayTao <= request.NgayKetThuc.Value);
            }
            int totalRow = await query.CountAsync();
            var data = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(h => new HoaDonClientViewModel
                {
                    Id = h.Id,
                    NgayTao = h.NgayTao,
                    TongTien = h.TongTien,
                    TrangThai = h.TrangThai,
                    HoTen = h.HoTen,
                    DiaChi = h.DiaChi,
                    SoDienThoai = h.SoDienThoai,
                    Email = h.Email,
                    PhuongThucThanhToan = h.PhuongThucThanhToan,
                    LoaiHoaDon = h.LoaiHoaDon,
                    NgayThanhToan = h.NgayThanhToan,
                    MaHoaDon = h.MaHoaDon,
                    PhiVanChuyen = h.PhiVanChuyen,
                    DonViVanChuyen = h.DonViVanChuyen,
                    MaVanDon = h.MaVanDon,
                    TrangThaiThanhToan = h.TrangThaiThanhToan,
                }).ToListAsync();
            var pagedResult = new PagedResult<HoaDonClientViewModel>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<HoaDonClientViewModel> GetById(Guid id)
        {
            var hoaDon = await _context.HoaDon
                .Include(h => h.HoaDonChiTiet)
                .ThenInclude(hdc => hdc.SanPhamChiTiet)
                .Include(h => h.User)
                .Include(h => h.Voucher)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (hoaDon == null) return null;
            return new HoaDonClientViewModel
            {
                Id = hoaDon.Id,
                NgayTao = hoaDon.NgayTao,
                TongTien = hoaDon.TongTien,
                TrangThai = hoaDon.TrangThai,
                HoTen = hoaDon.HoTen,
                DiaChi = hoaDon.DiaChi,
                SoDienThoai = hoaDon.SoDienThoai,
                Email = hoaDon.Email,
                PhuongThucThanhToan = hoaDon.PhuongThucThanhToan,
                LoaiHoaDon = hoaDon.LoaiHoaDon,
                NgayThanhToan = hoaDon.NgayThanhToan,
                MaHoaDon = hoaDon.MaHoaDon,
                PhiVanChuyen = hoaDon.PhiVanChuyen,
                DonViVanChuyen = hoaDon.DonViVanChuyen,
                MaVanDon = hoaDon.MaVanDon,
                TrangThaiThanhToan = hoaDon.TrangThaiThanhToan,
                VoucherId = hoaDon.VoucherId,
            };
        }

        public async Task<HoaDonClientViewModel> Create(ThemHoaDonClient request)
        {
            var maHoaDon = $"HD{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
            var hoaDon = new Data.Entities.HoaDon
            {
                Id = Guid.NewGuid(),
                NgayTao = DateTime.Now,
                TongTien = request.TongTien,
                TrangThai = request.TrangThai,
                UserId = request.UserId,
                DiaChi = request.DiaChi,
                SoDienThoai = request.SoDienThoai,
                Email = request.Email,
                HoTen = request.HoTen,
                GhiChu = request.GhiChu,
                PhuongThucThanhToan = request.PhuongThucThanhToan,
                LoaiHoaDon = request.LoaiHoaDon,
                NgayThanhToan = request.NgayThanhToan,
                MaHoaDon = maHoaDon,
                PhiVanChuyen = request.PhiVanChuyen,
                DonViVanChuyen = request.DonViVanChuyen,
                MaVanDon = request.MaVanDon,
                TrangThaiThanhToan = request.TrangThaiThanhToan,
                VoucherId = request.VoucherId
            };
            _context.HoaDon.Add(hoaDon);
            await _context.SaveChangesAsync();

            return await GetById(hoaDon.Id);
        }

        public async Task<HoaDonClientViewModel> Update(SuaHoaDonClient request)
        {
            var hoaDon = await _context.HoaDon.FindAsync(request.Id);
            if (hoaDon == null) return null;

            hoaDon.TongTien = request.TongTien;
            hoaDon.TrangThai = request.TrangThai;
            hoaDon.DiaChi = request.DiaChi;
            hoaDon.SoDienThoai = request.SoDienThoai;
            hoaDon.Email = request.Email;
            hoaDon.HoTen = request.HoTen;
            hoaDon.GhiChu = request.GhiChu;
            hoaDon.PhuongThucThanhToan = request.PhuongThucThanhToan;
            hoaDon.LoaiHoaDon = request.LoaiHoaDon;
            hoaDon.NgayThanhToan = request.NgayThanhToan;
            hoaDon.MaHoaDon = request.MaHoaDon;
            hoaDon.PhiVanChuyen = request.PhiVanChuyen;
            hoaDon.DonViVanChuyen = request.DonViVanChuyen;
            hoaDon.MaVanDon = request.MaVanDon;
            hoaDon.TrangThaiThanhToan = request.TrangThaiThanhToan;
            hoaDon.VoucherId = request.VoucherId;

            await _context.SaveChangesAsync();
            return await GetById(hoaDon.Id);
        }

        public async Task<Dictionary<TrangThaiHoaDon, int>> GetCountByStatusAsync()
        {
            // Lấy toàn bộ hóa đơn, group by trạng thái, đếm từng loại
            var counts = await _context.HoaDon
                .GroupBy(h => h.TrangThai)
                .Select(g => new { TrangThai = g.Key, Count = g.Count() })
                .ToListAsync();

            // Đưa về Dictionary cho dễ dùng
            return counts.ToDictionary(x => x.TrangThai, x => x.Count);
        }
    }
}
