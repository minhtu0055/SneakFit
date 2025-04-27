using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.MauSac;
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
        public async Task<List<MauSacViewModels>> GetAll()
        {
            var list = await _context.MauSac.Select(x => new MauSacViewModels()
            {
                Id = x.Id,
                TenMauSac = x.TenMauSac
            }).ToListAsync();
            return list;
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
                TenMauSac = getid.TenMauSac
            };
        }
        public async Task<MauSacViewModels> Create(ThemMauSac request)
        {
            var newMauSac = new Data.Entities.MauSac()
            {
                Id = Guid.NewGuid(),
                TenMauSac = request.TenMauSac
            };
            _context.MauSac.Add(newMauSac);
            await _context.SaveChangesAsync();
            return new MauSacViewModels()
            {
                Id = newMauSac.Id,
                TenMauSac = newMauSac.TenMauSac
            };
        }
        public async Task<MauSacViewModels> Update(SuaMauSac request)
        {
            var getid = _context.MauSac.Find(request.Id);
            if (getid == null)
            {
                throw new Exception($"Không tìm thấy id : {request.Id} của thương hiêu");
            }
            getid.TenMauSac = request.TenMauSac;
            await _context.SaveChangesAsync();
            return new MauSacViewModels()
            {
                Id = getid.Id,
                TenMauSac = getid.TenMauSac
            };
        }
    }
}
