using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.HoaDon
{
    public class HoaDonService : IHoaDonService
    {
        private readonly SneakFitDbContext _context;

        public HoaDonService(SneakFitDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<HoaDonViewModel>> GetAllPaging(PhanTrangHoaDon request)
        {
            var query = _context.HoaDon
                .Include(h => h.HoaDonChiTiet)
                .ThenInclude(hdc => hdc.SanPhamChiTiet)
                .Include(h => h.User)
                .Include(h => h.Voucher)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(h => h.HoTen.Contains(request.Keyword) || h.MaGiaoDich.Contains(request.Keyword));
            }

            var totalRecords = await query.CountAsync();
            var items = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(h => new HoaDonViewModel
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
                    MaGiaoDich = h.MaGiaoDich,
                    PhiVanChuyen = h.PhiVanChuyen,
                    DonViVanChuyen = h.DonViVanChuyen,
                    MaVanDon = h.MaVanDon,
                    TrangThaiThanhToan = h.TrangThaiThanhToan,
                    HoaDonChiTiet = h.HoaDonChiTiet.Select(hdc => new HoaDonChiTietViewModel
                    {
                        Id = hdc.Id,
                        SoLuong = hdc.SoLuong,
                        GiaBan = hdc.GiaBan,
                       // SanPhamChiTietName = hdc.SanPhamChiTiet.TenSanPham // Giả định
                    }).ToList()
                }).ToListAsync();

            return new PagedResult<HoaDonViewModel>
            {
                Items = items,
                TotalRecords = totalRecords,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
        }

        public async Task<HoaDonViewModel> GetById(Guid id)
        {
            var hoaDon = await _context.HoaDon
                .Include(h => h.HoaDonChiTiet)
                .ThenInclude(hdc => hdc.SanPhamChiTiet)
                .Include(h => h.User)
                .Include(h => h.Voucher)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hoaDon == null) return null;

            return new HoaDonViewModel
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
                MaGiaoDich = hoaDon.MaGiaoDich,
                PhiVanChuyen = hoaDon.PhiVanChuyen,
                DonViVanChuyen = hoaDon.DonViVanChuyen,
                MaVanDon = hoaDon.MaVanDon,
                TrangThaiThanhToan = hoaDon.TrangThaiThanhToan,
                HoaDonChiTiet = hoaDon.HoaDonChiTiet.Select(hdc => new HoaDonChiTietViewModel
                {
                    Id = hdc.Id,
                    SoLuong = hdc.SoLuong,
                    GiaBan = hdc.GiaBan,
                    //SanPhamChiTietName = hdc.SanPhamChiTiet.TenSanPham
                }).ToList()
            };
        }

        public async Task<HoaDonViewModel> Create(ThemHoaDon request)
        {
            var hoaDon = new SneakFit.Data.Entities.HoaDon
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
                MaGiaoDich = request.MaGiaoDich,
                PhiVanChuyen = request.PhiVanChuyen,
                DonViVanChuyen = request.DonViVanChuyen,
                MaVanDon = request.MaVanDon,
                TrangThaiThanhToan = request.TrangThaiThanhToan,
                VoucherID = request.VoucherID
            };

            _context.HoaDon.Add(hoaDon);
            await _context.SaveChangesAsync();

            return await GetById(hoaDon.Id);
        }

        public async Task<HoaDonViewModel> Update(SuaHoaDon request)
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
            hoaDon.MaGiaoDich = request.MaGiaoDich;
            hoaDon.PhiVanChuyen = request.PhiVanChuyen;
            hoaDon.DonViVanChuyen = request.DonViVanChuyen;
            hoaDon.MaVanDon = request.MaVanDon;
            hoaDon.TrangThaiThanhToan = request.TrangThaiThanhToan;
            hoaDon.VoucherID = request.VoucherID;

            await _context.SaveChangesAsync();
            return await GetById(hoaDon.Id);
        }

        public async Task<bool> UpdateStatus(Guid id, TrangThaiHoaDon trangThai)
        {
            var hoaDon = await _context.HoaDon.FindAsync(id);
            if (hoaDon == null) return false;

            hoaDon.TrangThai = trangThai;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
