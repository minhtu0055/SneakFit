using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.KichThuoc;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.KichThuoc
{
    public class KichThuocService : IKichThuocService
    {
        private readonly SneakFitDbContext _context;

        public KichThuocService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<KichThuocViewModels>> GetAllPaging(KichThuocPagingRequest request)
        {
            var query = _context.KichThuoc.AsQueryable();
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.MaKichThuoc.ToString().Equals(request.Keyword));
            }
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new KichThuocViewModels()
                {
                    Id = x.Id,
                    MaKichThuoc = x.MaKichThuoc
                }).ToListAsync();
            var PageResult = new PagedResult<KichThuocViewModels>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data,
            };
            return PageResult;
        }
        public async Task<KichThuocViewModels> GetById(Guid id)
        {
            var KichThuoc = await _context.KichThuoc.FindAsync(id);
            if (KichThuoc == null) throw new KeyNotFoundException("Không tìm thấy đôi giày!");

            return new KichThuocViewModels { Id = KichThuoc.Id, MaKichThuoc = KichThuoc.MaKichThuoc };
        }

        public async Task<KichThuocViewModels> Create(ThemKichThuoc request)
        {
            // Kiểm tra xem mã kích thước đã tồn tại chưa
            var existingKichThuoc = await _context.KichThuoc
            .FirstOrDefaultAsync(x => x.MaKichThuoc == request.MaKichThuoc);
            if (existingKichThuoc != null)
            {
                throw new Exception("Mã kích thước đã tồn tại!");
            }
            var newKichThuoc = new Data.Entities.KichThuoc()
            {
                Id = Guid.NewGuid(),
                MaKichThuoc = request.MaKichThuoc
            };
            _context.KichThuoc.Add(newKichThuoc);
            await _context.SaveChangesAsync();
            return new KichThuocViewModels()
            {
                Id = newKichThuoc.Id,
                MaKichThuoc = newKichThuoc.MaKichThuoc
            };
        }

        public async Task<KichThuocViewModels> Update(SuaKichThuoc request)
        {
            var getid = _context.KichThuoc.Find(request.Id);
            if (getid == null)
            {
                throw new Exception($"Không tìm thấy id : {request.Id} của thương hiêu");
            }
            // Kiểm tra xem tên chất liệu đã tồn tại chưa (trừ chính bản ghi hiện tại)
            var existingKichThuoc = await _context.KichThuoc
                .FirstOrDefaultAsync(x => x.MaKichThuoc == request.MaKichThuoc
                                        && x.Id != request.Id);
            if (existingKichThuoc != null)
            {
                throw new Exception("Tên chất liệu đã tồn tại!");
            }
            getid.MaKichThuoc = request.MaKichThuoc;
            await _context.SaveChangesAsync();
            return new KichThuocViewModels()
            {
                Id = getid.Id,
                MaKichThuoc = getid.MaKichThuoc
            };
        }
    }
}
