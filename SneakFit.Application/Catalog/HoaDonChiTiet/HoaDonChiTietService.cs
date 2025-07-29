using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.Data.Entities;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.HoaDonChiTiet
{
    public class HoaDonChiTietService : IHoaDonChiTietService
    {
        private readonly SneakFitDbContext _context;

        public HoaDonChiTietService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<HoaDonChiTietViewModel>> GetAllPaging(PhanTrangHoaDonChiTiet request)
        {
            var query = _context.HoaDonChiTiet
                .Include(h => h.HoaDon)
                .Include(h => h.SanPhamChiTiet)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.HoaDon.MaHoaDon.Contains(request.Keyword));
            }

            int totalRow = await query.CountAsync();
            var data = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new HoaDonChiTietViewModel()
                {
                    Id = x.Id,
                    SoLuong = x.SoLuong,
                    GiaBan = x.GiaBan
                }).ToListAsync();
            var pagedResult = new PagedResult<HoaDonChiTietViewModel>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<List<HoaDonChiTietViewModel>> GetById(Guid id)
        {
            var chiTiets = await _context.HoaDonChiTiet
                .Where(x => x.HoaDonId == id)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.SanPham)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.KichThuoc)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.MauSac)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.HinhAnhSanPham)
                .ToListAsync();

            return chiTiets.Select(hoaDonChiTiet => new HoaDonChiTietViewModel()
            {
                Id = hoaDonChiTiet.Id,
                SoLuong = hoaDonChiTiet.SoLuong,
                GiaBan = hoaDonChiTiet.GiaBan,
                SanPhamChiTietId = hoaDonChiTiet.SanPhamChiTietId,
                TenSanPham = hoaDonChiTiet.SanPhamChiTiet?.SanPham?.TenSanPham,
                KichThuoc = hoaDonChiTiet.SanPhamChiTiet?.KichThuoc?.MaKichThuoc,
                MauSac = hoaDonChiTiet.SanPhamChiTiet?.MauSac?.TenMauSac,
                SoLuongTon = hoaDonChiTiet.SanPhamChiTiet?.SoLuong ?? 0,
                Images = hoaDonChiTiet.SanPhamChiTiet?.HinhAnhSanPham?.Select(x => x.UrlHinhAnh).ToList() ?? new List<string>()
            }).ToList();
        }

        public async Task<HoaDonChiTietViewModel> CreateOrUpdate(ThemHoaDonChiTiet request)
        {
            var spct = await _context.SanPhamChiTiet.FirstOrDefaultAsync(x => x.ID == request.SanPhamChiTietId);
            if (spct == null) throw new Exception("Không tìm thấy sản phẩm chi tiết");
            var existing = await _context.HoaDonChiTiet
                .FirstOrDefaultAsync(x => x.HoaDonId == request.HoaDonId && x.SanPhamChiTietId == request.SanPhamChiTietId);

            if (existing != null)
            {
                if (spct.SoLuong < request.SoLuong)
                    throw new Exception("Số lượng tồn kho không đủ");

                existing.SoLuong += request.SoLuong;
                existing.GiaBan = request.GiaBan;
                spct.SoLuong -= request.SoLuong;
                await _context.SaveChangesAsync();
                return new HoaDonChiTietViewModel
                {
                    Id = existing.Id,
                    SoLuong = existing.SoLuong,
                    GiaBan = existing.GiaBan,
                    SanPhamChiTietId = existing.SanPhamChiTietId
                };
            }
            else
            {
                if (spct.SoLuong < request.SoLuong)
                    throw new Exception("Số lượng tồn kho không đủ");

                var hoaDonChiTiet = new Data.Entities.HoaDonChiTiet()
                {
                    Id = Guid.NewGuid(),
                    SoLuong = request.SoLuong,
                    GiaBan = request.GiaBan,
                    HoaDonId = request.HoaDonId,
                    SanPhamChiTietId = request.SanPhamChiTietId
                };
                _context.HoaDonChiTiet.Add(hoaDonChiTiet);
                spct.SoLuong -= request.SoLuong;
                await _context.SaveChangesAsync();
                return new HoaDonChiTietViewModel
                {
                    Id = hoaDonChiTiet.Id,
                    SoLuong = hoaDonChiTiet.SoLuong,
                    GiaBan = hoaDonChiTiet.GiaBan,
                    SanPhamChiTietId = hoaDonChiTiet.SanPhamChiTietId
                };
            }
        }

        public async Task<HoaDonChiTietViewModel> Edit(SuaHoaDonChiTiet request)
        {
            var hoaDonChiTiet = await _context.HoaDonChiTiet.FindAsync(request.Id);
            if (hoaDonChiTiet == null) return null;
            hoaDonChiTiet.SoLuong = request.SoLuong;
            hoaDonChiTiet.GiaBan = request.GiaBan;
            await _context.SaveChangesAsync();
            return new HoaDonChiTietViewModel
            {
                Id = hoaDonChiTiet.Id,
                SoLuong = hoaDonChiTiet.SoLuong,
                GiaBan = hoaDonChiTiet.GiaBan,
            };
        }

        public async Task<bool> Delete(Guid id)
        {
            var hoaDonChiTiet = await _context.HoaDonChiTiet.FindAsync(id);
            if (hoaDonChiTiet == null) return false;
            
            var spct = await _context.SanPhamChiTiet.FirstOrDefaultAsync(x => x.ID == hoaDonChiTiet.SanPhamChiTietId);
            if (spct != null)
                spct.SoLuong += hoaDonChiTiet.SoLuong;

            _context.HoaDonChiTiet.Remove(hoaDonChiTiet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateQuantity(Guid hoaDonChiTietId, int newQuantity)
        {
            var hoaDonChiTiet = await _context.HoaDonChiTiet
                .Include(x => x.SanPhamChiTiet)
                .FirstOrDefaultAsync(x => x.Id == hoaDonChiTietId);

            if (hoaDonChiTiet == null) throw new Exception("Không tìm thấy hóa đơn chi tiết");

            var spct = hoaDonChiTiet.SanPhamChiTiet;
            if (spct == null) throw new Exception("Không tìm thấy sản phẩm chi tiết");

            int oldQuantity = hoaDonChiTiet.SoLuong;
            int diff = newQuantity - oldQuantity;

            if (diff == 0) return true; // Không thay đổi

            if (diff > 0)
            {
                // Tăng số lượng: kiểm tra kho
                if (spct.SoLuong < diff)
                    throw new Exception("Số lượng tồn kho không đủ");
                hoaDonChiTiet.SoLuong = newQuantity;
                spct.SoLuong -= diff;
            }
            else
            {
                // Giảm số lượng: trả lại kho
                hoaDonChiTiet.SoLuong = newQuantity;
                spct.SoLuong += -diff;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
