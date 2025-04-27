using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.KichThuoc;
using SneakFit.ViewModels.Catalog.ThuongHieu;
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

        public async Task<List<KichThuocViewModels>> GetAll()
        {
            return await _context.KichThuoc
                .Select(x => new KichThuocViewModels { Id = x.Id, MaKichThuoc = x.MaKichThuoc })
                .ToListAsync();
        }

        public async Task<KichThuocViewModels> GetById(Guid id)
        {
            var KichThuoc = await _context.KichThuoc.FindAsync(id);
            if (KichThuoc == null) throw new KeyNotFoundException("Không tìm thấy đôi giày!");

            return new KichThuocViewModels { Id = KichThuoc.Id, MaKichThuoc = KichThuoc.MaKichThuoc };
        }

        public async Task<KichThuocViewModels> Create(ThemKichThuoc request)
        {
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
