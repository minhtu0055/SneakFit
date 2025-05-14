using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.DeGiay;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.DeGiay
{
    public class DeGiayService : IDeGiayService
    {
        private readonly SneakFitDbContext _context;

        public DeGiayService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<DeGiayViewModels>> GetAllPaging(DeGiayPagingRequest request)
        {
            var query = _context.DeGiay.AsQueryable();
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.TenDeGiay.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new DeGiayViewModels()
                {
                    Id = x.Id,
                    TenDeGiay = x.TenDeGiay
                }).ToListAsync();
            var PageResult = new PagedResult<DeGiayViewModels>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data,
            };
            return PageResult;
        }
        public async Task<DeGiayViewModels> GetById(Guid id)
        {
            var getid = await _context.DeGiay.FindAsync(id);
            if (getid == null)
            {
                throw new Exception($"Không tìm thấy id : {id} của thương hiêu");
            }
            return new DeGiayViewModels()
            {
                Id = getid.Id,
                TenDeGiay = getid.TenDeGiay
            };
        }
        public async Task<DeGiayViewModels> Create(ThemDeGiay request)
        {
            // Kiểm tra xem tên chất liệu đã tồn tại chưa
            var existingChatLieu = await _context.DeGiay
                .FirstOrDefaultAsync(x => x.TenDeGiay.ToLower() == request.TenDeGiay.ToLower());
            if (existingChatLieu != null)
            {
                throw new Exception("Tên chất liệu đã tồn tại!");
            }
            var newDeGiay = new Data.Entities.DeGiay()
            {
                Id = Guid.NewGuid(),
                TenDeGiay = request.TenDeGiay
            };
            _context.DeGiay.Add(newDeGiay);
            await _context.SaveChangesAsync();
            return new DeGiayViewModels()
            {
                Id = newDeGiay.Id,
                TenDeGiay = newDeGiay.TenDeGiay
            };
        }
        public async Task<DeGiayViewModels> Update(SuaDeGiay request)
        {
            var getid = _context.DeGiay.Find(request.Id);
            if (getid == null)
            {
                throw new Exception($"Không tìm thấy id : {request.Id} của thương hiêu");
            }
            // Kiểm tra xem tên chất liệu đã tồn tại chưa (trừ chính bản ghi hiện tại)
            var existingChatLieu = await _context.DeGiay
                .FirstOrDefaultAsync(x => x.TenDeGiay.ToLower() == request.TenDeGiay.ToLower()
                                        && x.Id != request.Id);
            if (existingChatLieu != null)
            {
                throw new Exception("Tên chất liệu đã tồn tại!");
            }
            getid.TenDeGiay = request.TenDeGiay;
            await _context.SaveChangesAsync();
            return new DeGiayViewModels()
            {
                Id = getid.Id,
                TenDeGiay = getid.TenDeGiay
            };
        }


    }
}
