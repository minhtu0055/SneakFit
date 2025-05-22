using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.DanhMuc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.DanhMuc
{
    public class DanhMucService : IDanhMucService
    {
        private readonly SneakFitDbContext _context;

        public DanhMucService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<List<DanhMucViewModels>> GetAll()
        {
            var list = await _context.DanhMuc.Select(x => new DanhMucViewModels()
            {
                Id = x.Id,
                TenDanhMuc = x.TenDanhMuc,
                SoSanPham = _context.SanPham.Count(s => s.DanhMucId == x.Id && s.TrangThai == true)
            }).ToListAsync();
            return list;
        }
        public async Task<DanhMucViewModels> GetById(Guid id)
        {
            var getid = await _context.DanhMuc.FindAsync(id);
            if (getid == null)
            {
                throw new Exception($"Không tìm thấy id : {id} của thương hiêu");
            }
            return new DanhMucViewModels()
            {
                Id = getid.Id,
                TenDanhMuc = getid.TenDanhMuc,
                SoSanPham = await _context.SanPham.CountAsync(s => s.DanhMucId == getid.Id && s.TrangThai == true)
            };
        }
        public async Task<DanhMucViewModels> Create(ThemDanhMuc request)
        {
            var newDanhMuc = new Data.Entities.DanhMuc()
            {
                Id = Guid.NewGuid(),
                TenDanhMuc = request.TenDanhMuc
            };
            _context.DanhMuc.Add(newDanhMuc);
            await _context.SaveChangesAsync();
            return new DanhMucViewModels()
            {
                Id = newDanhMuc.Id,
                TenDanhMuc = newDanhMuc.TenDanhMuc,
                SoSanPham = 0
            };
        }
        public async Task<DanhMucViewModels> Update(SuaDanhMuc request)
        {
            var getid = await _context.DanhMuc.FindAsync(request.Id);
            if (getid == null)
            {
                throw new Exception($"Không tìm thấy id : {request.Id} của thương hiêu");
            }
            getid.TenDanhMuc = request.TenDanhMuc;
            await _context.SaveChangesAsync();
            return new DanhMucViewModels()
            {
                Id = getid.Id,
                TenDanhMuc = getid.TenDanhMuc,
                SoSanPham = await _context.SanPham.CountAsync(s => s.DanhMucId == getid.Id && s.TrangThai == true)
            };
        }
    }
}
