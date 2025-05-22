using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.MauSac
{
    public class MauSacService : IMauSacService
    {
        private readonly SneakFitDbContext _context;

        public MauSacService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<MauSacViewModels>> GetAllPaging(MauSacPagingRequest request)
        {
            var query = _context.MauSac.AsQueryable();
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.TenMauSac.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new MauSacViewModels()
                {
                    Id = x.Id,
                    TenMauSac = x.TenMauSac,
                    MaMauSac = x.MaMauSac
                }).ToListAsync();
            var PageResult = new PagedResult<MauSacViewModels>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data,
            };
            return PageResult;
        }
        public async Task<MauSacViewModels> GetById(Guid id)
        {
            var getid = await _context.MauSac.FindAsync(id);
            if (getid == null)
            {
                throw new Exception($"Không tìm thấy id : {id} của thương hiêu");
            }
            return new MauSacViewModels()
            {
                Id = getid.Id,
                TenMauSac = getid.TenMauSac,
                MaMauSac = getid.MaMauSac
            };
        }
        public async Task<MauSacViewModels> Create(ThemMauSac request)
        {
            // Kiểm tra xem tên chất liệu đã tồn tại chưa
            var existingChatLieu = await _context.MauSac
                .FirstOrDefaultAsync(x => x.TenMauSac.ToLower() == request.TenMauSac.ToLower());
            if (existingChatLieu != null)
            {
                throw new Exception("Tên màu sắc đã tồn tại!");
            }
            var newMauSac = new Data.Entities.MauSac()
            {
                Id = Guid.NewGuid(),
                TenMauSac = request.TenMauSac,
                MaMauSac = request.MaMauSac
            };
            _context.MauSac.Add(newMauSac);
            await _context.SaveChangesAsync();
            return new MauSacViewModels()
            {
                Id = newMauSac.Id,
                TenMauSac = newMauSac.TenMauSac,
                MaMauSac = newMauSac.MaMauSac
            };
        }
        public async Task<MauSacViewModels> Update(SuaMauSac request)
        {
            var getid = _context.MauSac.Find(request.Id);
            if (getid == null)
            {
                throw new Exception($"Không tìm thấy id : {request.Id} của thương hiêu");
            }
            // Kiểm tra xem tên chất liệu đã tồn tại chưa (trừ chính bản ghi hiện tại)
            var existingChatLieu = await _context.MauSac
                .FirstOrDefaultAsync(x => x.TenMauSac.ToLower() == request.TenMauSac.ToLower()
                                        && x.Id != request.Id);
            if (existingChatLieu != null)
            {
                throw new Exception("Tên màu sắc đã tồn tại!");
            }
            getid.TenMauSac = request.TenMauSac;
            await _context.SaveChangesAsync();
            return new MauSacViewModels()
            {
                Id = getid.Id,
                TenMauSac = getid.TenMauSac,
                MaMauSac = getid.MaMauSac
            };
        }
        public async Task<List<MauSacViewModels>> GetAll()
        {
            return await _context.MauSac.Select(x => new MauSacViewModels
            {
                Id = x.Id,
                TenMauSac = x.TenMauSac,
                MaMauSac = x.MaMauSac
            }).ToListAsync();
        }
    }
}
