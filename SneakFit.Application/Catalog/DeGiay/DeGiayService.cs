using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.DeGiay;
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
        public async Task<List<DeGiayViewModels>> GetAll()
        {
            var list = await _context.DeGiay.Select(x => new DeGiayViewModels()
            {
                Id = x.Id,
                TenDeGiay = x.TenDeGiay
            }).ToListAsync();
            return list;
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
