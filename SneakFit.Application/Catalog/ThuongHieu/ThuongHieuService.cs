using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.Data.Entities;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.ThuongHieu;
using SneakFit.ViewModels.Common;

namespace SneakFit.Application.Catalog.ThuongHieu
{
    public class ThuongHieuService : IThuongHieuService
    {
        private readonly SneakFitDbContext _context;

        public ThuongHieuService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<ThuongHieuViewModels>> GetAllPaging(ThuongHieuPagingRequest request)
        {
            var query = _context.ThuongHieu.AsQueryable();
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.TenThuongHieu.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ThuongHieuViewModels()
                {
                    Id = x.Id,
                    TenThuongHieu = x.TenThuongHieu
                }).ToListAsync();
            var PageResult = new PagedResult<ThuongHieuViewModels>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data,
            };
            return PageResult;
        }
        public async Task<List<ThuongHieuViewModels>> GetAll()
        {
            var list = await _context.ThuongHieu.Select(x => new ThuongHieuViewModels()
            {
                Id = x.Id,
                TenThuongHieu = x.TenThuongHieu
            }).ToListAsync();
            return list;
        }
        public async Task<ThuongHieuViewModels> GetById(Guid id)
        {
            var getid = await _context.ThuongHieu.FindAsync(id);
            if(getid == null)
            {
                throw new Exception($"Không tìm thấy id : {id} của thương hiêu");
            }
            return new ThuongHieuViewModels()
            {
                Id = getid.Id,
                TenThuongHieu = getid.TenThuongHieu
            };
        }
        public async Task<ThuongHieuViewModels> Create(ThemThuongHieu request)
        {
            var newThuongHieu = new Data.Entities.ThuongHieu()
            {
                Id = Guid.NewGuid(),
                TenThuongHieu = request.TenThuongHieu
            };
            _context.ThuongHieu.Add(newThuongHieu);
            await _context.SaveChangesAsync();
            return new ThuongHieuViewModels()
            {
                Id = newThuongHieu.Id,
                TenThuongHieu = newThuongHieu.TenThuongHieu
            };
        }
        public async Task<ThuongHieuViewModels> Update(SuaThuongHieu request)
        {
            var getid = _context.ThuongHieu.Find(request.Id);
            if(getid == null)
            {
                throw new Exception($"Không tìm thấy id : {request.Id} của thương hiêu");
            }
            getid.TenThuongHieu = request.TenThuongHieu;
            await _context.SaveChangesAsync();
            return new ThuongHieuViewModels()
            {
                Id = getid.Id,
                TenThuongHieu = getid.TenThuongHieu
            };
        }
    }
}
