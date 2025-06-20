using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
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
                query = query.Where(x => x.HoaDon.MaGiaoDich.Contains(request.Keyword));
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

        public async Task<HoaDonChiTietViewModel> GetById(Guid id)
        {
            var hoaDonChiTiet = await _context.HoaDonChiTiet
                .Include(h => h.HoaDon)
                .Include(h => h.SanPhamChiTiet)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (hoaDonChiTiet == null) return null;

            return new HoaDonChiTietViewModel()
            {
                Id = hoaDonChiTiet.Id,
                SoLuong = hoaDonChiTiet.SoLuong,
                GiaBan = hoaDonChiTiet.GiaBan
            };
        }

        public async Task<HoaDonChiTietViewModel> Create(ThemHoaDonChiTiet request)
        {
            var hoaDonChiTiet = new Data.Entities.HoaDonChiTiet()
            {
                Id = Guid.NewGuid(),
                SoLuong = request.SoLuong,
                GiaBan = request.GiaBan,
                HoaDonId = request.HoaDonId,
                SanPhamChiTietId = request.SanPhamChiTietId
            };

            _context.HoaDonChiTiet.Add(hoaDonChiTiet);
            await _context.SaveChangesAsync();

            return await GetById(hoaDonChiTiet.Id);
        }

        public async Task<HoaDonChiTietViewModel> Edit(SuaHoaDonChiTiet request)
        {
            var hoaDonChiTiet = await _context.HoaDonChiTiet.FindAsync(request.Id);
            if (hoaDonChiTiet == null) return null;

            hoaDonChiTiet.SoLuong = request.SoLuong;
            hoaDonChiTiet.GiaBan = request.GiaBan;

            await _context.SaveChangesAsync();
            return await GetById(hoaDonChiTiet.Id);
        }
    }
}
