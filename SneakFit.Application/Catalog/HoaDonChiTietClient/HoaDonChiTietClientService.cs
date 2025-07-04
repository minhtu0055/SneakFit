using Microsoft.EntityFrameworkCore;
using SneakFit.Application.Catalog.HoaDonChiTietClients;
using SneakFit.Data.EF;
using SneakFit.Data.Entities;
using SneakFit.ViewModels.Catalog.HoaDonChiTietClient;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.HoaDonChiTietClientClient
{
    public class HoaDonChiTietClientService : IHoaDonChiTietClientService
    {
        private readonly SneakFitDbContext _context;

        public HoaDonChiTietClientService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<HoaDonChiTietClientViewModel>> GetAllPaging(PhanTrangHoaDonChiTietClient request)
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
                .Select(x => new HoaDonChiTietClientViewModel()
                {
                    Id = x.Id,
                    SoLuong = x.SoLuong,
                    GiaBan = x.GiaBan
                }).ToListAsync();
            var pagedResult = new PagedResult<HoaDonChiTietClientViewModel>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<List<HoaDonChiTietClientViewModel>> GetById(Guid id)
        {
            var chiTiets = await _context.HoaDonChiTiet
                .Where(x => x.HoaDonId == id)
                .Include(h => h.HoaDon)
                .Include(h => h.SanPhamChiTiet)
                .ToListAsync();

            return chiTiets.Select(HoaDonChiTietClient => new HoaDonChiTietClientViewModel()
            {
                Id = HoaDonChiTietClient.Id,
                SoLuong = HoaDonChiTietClient.SoLuong,
                GiaBan = HoaDonChiTietClient.GiaBan
                // Thêm các trường khác nếu cần
            }).ToList();
        }

        public async Task<HoaDonChiTietClientViewModel> Create(ThemHoaDonChiTietClient request)
        {
            var HoaDonChiTietClient = new Data.Entities.HoaDonChiTiet()
            {
                Id = Guid.NewGuid(),
                SoLuong = request.SoLuong,
                GiaBan = request.GiaBan,
                HoaDonId = request.HoaDonId,
                SanPhamChiTietId = request.SanPhamChiTietId
            };
            _context.HoaDonChiTiet.Add(HoaDonChiTietClient);
            await _context.SaveChangesAsync();
            return new HoaDonChiTietClientViewModel
            {
                Id = HoaDonChiTietClient.Id,
                SoLuong = HoaDonChiTietClient.SoLuong,
                GiaBan = HoaDonChiTietClient.GiaBan,
            };
        }

        public async Task<HoaDonChiTietClientViewModel> Edit(SuaHoaDonChiTietClient request)
        {
            var HoaDonChiTietClient = await _context.HoaDonChiTiet.FindAsync(request.Id);
            if (HoaDonChiTietClient == null) return null;
            HoaDonChiTietClient.SoLuong = request.SoLuong;
            HoaDonChiTietClient.GiaBan = request.GiaBan;
            await _context.SaveChangesAsync();
            return new HoaDonChiTietClientViewModel
            {
                Id = HoaDonChiTietClient.Id,
                SoLuong = HoaDonChiTietClient.SoLuong,
                GiaBan = HoaDonChiTietClient.GiaBan,
            };
        }
    }
}
