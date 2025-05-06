using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPham;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.SanPham
{
    public class SanPhamService : ISanPhamService
    {
        private readonly SneakFitDbContext _context;

        public SanPhamService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<List<SanPhamViewModels>> GetAll()
        {
            var list = await _context.SanPham
                .Include(x => x.DanhMuc)
                .Select(x => new SanPhamViewModels()
                {
                    Id = x.Id,
                    TenSanPham = x.TenSanPham,
                    Mota = x.Mota,
                    DanhMucId = x.DanhMucId,
                    TenDanhMuc = x.DanhMuc.TenDanhMuc
                })
                .ToListAsync();

            return list;
        }

        public async Task<SanPhamViewModels?> GetById(Guid id)
        {
            var entity = await _context.SanPham
                .Include(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return null;

            return new SanPhamViewModels()
            {
                Id = entity.Id,
                TenSanPham = entity.TenSanPham,
                Mota = entity.Mota,
                DanhMucId = entity.DanhMucId,
                TenDanhMuc = entity.DanhMuc.TenDanhMuc
            };
        }

        public async Task<SanPhamViewModels> Create(ThemSanPham request)
        {
            var danhMuc = await _context.DanhMuc.FindAsync(request.DanhMucId);
            if (danhMuc == null)
                throw new Exception($"Không tìm thấy danh mục với id = {request.DanhMucId}");

            var newSanPham = new Data.Entities.SanPham()
            {
                Id = Guid.NewGuid(),
                TenSanPham = request.TenSanPham,
                Mota = request.Mota,
                DanhMucId = request.DanhMucId
            };

            _context.SanPham.Add(newSanPham);

            await _context.SaveChangesAsync();

            return new SanPhamViewModels()
            {
                Id = newSanPham.Id,
                TenSanPham = newSanPham.TenSanPham,
                Mota = newSanPham.Mota,
                DanhMucId = newSanPham.DanhMucId,
                TenDanhMuc = danhMuc.TenDanhMuc
            };
        }


        public async Task<SanPhamViewModels?> Update(SuaSanPham request)
        {
            var entity = await _context.SanPham
                .Include(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (entity == null)
                return null;

            entity.TenSanPham = request.TenSanPham;
            entity.Mota = request.Mota ?? entity.Mota;
            entity.DanhMucId = request.DanhMucId;

            await _context.SaveChangesAsync();

            entity = await _context.SanPham
                .Include(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            return new SanPhamViewModels()
            {
                Id = entity.Id,
                TenSanPham = entity.TenSanPham,
                Mota = entity.Mota,
                DanhMucId = entity.DanhMucId,
                TenDanhMuc = entity.DanhMuc?.TenDanhMuc
            };
        }
    }
}
