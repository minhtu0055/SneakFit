using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.DiaChi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.System.DiaChi
{
    public class DiaChiService : IDiaChiService
    {
        private readonly SneakFitDbContext _context;

        public DiaChiService(SneakFitDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<List<DiaChiViewModel>>> GetAllByUser(Guid userId)
        {
            var diaChis = await _context.DiaChi
                .Where(x => x.UserId == userId)
                .Select(x => new DiaChiViewModel()
                {
                    Id = x.Id,
                    TenDiaChi = x.TenDiaChi,
                    TenNguoiNhan = x.TenNguoiNhan,
                    SoDienThoai = x.SoDienThoai,
                    TenThanhPho = x.TenThanhPho,
                    TenHuyen = x.TenHuyen,
                    TenXa = x.TenXa,
                    MacDinh = x.Mac_Dinh,
                    MaTinh = x.MaTinh,
                    MaHuyen = x.MaHuyen,
                    MaXa = x.MaXa
                }).ToListAsync();

            return new ApiSuccessResult<List<DiaChiViewModel>>(diaChis);
        }

        public async Task<ApiResult<bool>> Create(Guid userId, ThemDiaChiViewModel request)
        {
            var diaChi = new SneakFit.Data.Entities.DiaChi()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SoDienThoai = request.SoDienThoai,
                TenNguoiNhan = request.TenNguoiNhan,
                TenDiaChi = request.TenDiaChi,
                TenThanhPho = request.TenThanhPho,
                TenHuyen = request.TenHuyen,
                TenXa = request.TenXa,
                Mac_Dinh = request.MacDinh,
                MaTinh = request.MaTinh,
                MaHuyen = request.MaHuyen,
                MaXa = request.MaXa
            };

            // Nếu đây là địa chỉ mặc định, cập nhật các địa chỉ khác
            if (request.MacDinh)
            {
                var existingAddresses = await _context.DiaChi
                    .Where(x => x.UserId == userId)
                    .ToListAsync();

                foreach (var address in existingAddresses)
                {
                    address.Mac_Dinh = false;
                }
            }
            // Nếu đây là địa chỉ đầu tiên, đặt làm mặc định
            else if (!await _context.DiaChi.AnyAsync(x => x.UserId == userId))
            {
                diaChi.Mac_Dinh = true;
            }
            _context.DiaChi.Add(diaChi);
            await _context.SaveChangesAsync();

            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> SetDefault(Guid id, Guid userId)
        {
            var diaChi = await _context.DiaChi
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (diaChi == null)
                return new ApiErrorResult<bool>("Địa chỉ không tồn tại");

            // Cập nhật tất cả địa chỉ thành không mặc định
            var allAddresses = await _context.DiaChi
                .Where(x => x.UserId == userId)
                .ToListAsync();

            foreach (var address in allAddresses)
            {
                address.Mac_Dinh = false;
            }

            // Đặt địa chỉ được chọn thành mặc định
            diaChi.Mac_Dinh = true;
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
        }
        public async Task<ApiResult<DiaChiViewModel>> GetById(Guid id, Guid userId)
        {
            var diaChi = await _context.DiaChi
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (diaChi == null)
                return new ApiErrorResult<DiaChiViewModel>("Địa chỉ không tồn tại");

            var diaChiVm = new DiaChiViewModel()
            {
                Id = diaChi.Id,
                TenDiaChi = diaChi.TenDiaChi,
                TenNguoiNhan = diaChi.TenNguoiNhan,
                SoDienThoai = diaChi.SoDienThoai,
                TenThanhPho = diaChi.TenThanhPho,
                TenHuyen = diaChi.TenHuyen,
                TenXa = diaChi.TenXa,
                MacDinh = diaChi.Mac_Dinh,
                MaTinh = diaChi.MaTinh,
                MaHuyen = diaChi.MaHuyen,
                MaXa = diaChi.MaXa
            };

            return new ApiSuccessResult<DiaChiViewModel>(diaChiVm);
        }

        public async Task<ApiResult<bool>> Update(Guid id, Guid userId, SuaDiaChiViewModel request)
        {
            var diaChi = await _context.DiaChi
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (diaChi == null)
                return new ApiErrorResult<bool>("Địa chỉ không tồn tại");

            diaChi.TenDiaChi = request.TenDiaChi;
            diaChi.SoDienThoai = request.SoDienThoai;
            diaChi.TenNguoiNhan = request.TenNguoiNhan;
            diaChi.TenThanhPho = request.TenThanhPho;
            diaChi.TenHuyen = request.TenHuyen;
            diaChi.TenXa = request.TenXa;
            diaChi.MaTinh = request.MaTinh;
            diaChi.MaHuyen = request.MaHuyen;
            diaChi.MaXa = request.MaXa;

            // Nếu đặt làm địa chỉ mặc định
            if (request.MacDinh && !diaChi.Mac_Dinh)
            {
                var existingAddresses = await _context.DiaChi
                    .Where(x => x.UserId == userId)
                    .ToListAsync();

                foreach (var address in existingAddresses)
                {
                    address.Mac_Dinh = false;
                }
                diaChi.Mac_Dinh = true;
            }

            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> Delete(Guid id, Guid userId)
        {
            var diaChi = await _context.DiaChi
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (diaChi == null)
                return new ApiErrorResult<bool>("Địa chỉ không tồn tại");

            // Nếu xóa địa chỉ mặc định, đặt địa chỉ đầu tiên (nếu có) làm mặc định
            if (diaChi.Mac_Dinh)
            {
                var firstAddress = await _context.DiaChi
                    .Where(x => x.UserId == userId && x.Id != id)
                    .FirstOrDefaultAsync();

                if (firstAddress != null)
                {
                    firstAddress.Mac_Dinh = true;
                }
            }

            _context.DiaChi.Remove(diaChi);
            await _context.SaveChangesAsync();

            return new ApiSuccessResult<bool>();
        }
    }
}
