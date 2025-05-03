using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.Data.Entities;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.VoucherCATA;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.VoucherRP
{
    public class VoucherService : IVoucherService
    {
        public readonly SneakFitDbContext _context;
        public VoucherService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<VoucherViewModels> Create(CreateVoucher request)
        {
            var vc = new Voucher()
            {
                Id = Guid.NewGuid(),
                MaVoucher = request.MaVoucher,
                LoaiGiamGia = request.LoaiGiamGia,
                GiaTriGiamGia = request.GiaTriGiamGia,
                DieuKienApDung = request.GiaTriGiamGia,
                SoLuong = request.SoLuong,
                NgayTao = DateTime.Now,
                ThoiGianBatDau = request.ThoiGianBatDau,
                ThoiGianKetThuc = request.ThoiGianKetThuc,
                TrangThai = DateTime.Now >= request.ThoiGianBatDau ? TrangThaiGiamGia.HoatDong : TrangThaiGiamGia.HetHan,
            };
            _context.Voucher.Add(vc);
            await _context.SaveChangesAsync();
            return await GetById(vc.Id);
        }
        public async Task<PagedResult<VoucherViewModels>> GetAllPaging(GetVoucherPagingRequest request)
        {
            var query = _context.Voucher.AsQueryable();
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.MaVoucher.Contains(request.Keyword));
            }
            if (request.Status.HasValue)
            {
                query = query.Where(x => x.TrangThai == request.Status.Value);
            }
            int totalRow = await query.CountAsync();
            var dt = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new VoucherViewModels()
                {
                    Id = x.Id,
                    MaVoucher = x.MaVoucher,
                    LoaiGiamGia = x.LoaiGiamGia,
                    GiaTriGiamGia = x.GiaTriGiamGia,
                    DieuKienApDung = x.DieuKienApDung,
                    SoLuong = x.SoLuong,
                    NgayTao = x.NgayTao,
                    ThoiGianBatDau = x.ThoiGianBatDau,
                    ThoiGianKetThuc = x.ThoiGianKetThuc,
                    TrangThai = x.TrangThai
                }).ToListAsync();
            var PageResult = new PagedResult<VoucherViewModels>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = dt,
            };
            return PageResult;
        }
        public async Task<VoucherViewModels> GetByCode(string code)
        {
            var voucher = await _context.Voucher.FirstOrDefaultAsync(x => x.MaVoucher == code);
            if (voucher == null) throw new Exception($"Không tìm thấy voucher có mã: {code}");
            return new VoucherViewModels
            {
                Id = voucher.Id,
                MaVoucher = voucher.MaVoucher,
                LoaiGiamGia = voucher.LoaiGiamGia,
                GiaTriGiamGia = voucher.GiaTriGiamGia,
                DieuKienApDung = voucher.DieuKienApDung,
                SoLuong = voucher.SoLuong,
                NgayTao = voucher.NgayTao,
                ThoiGianBatDau = voucher.ThoiGianBatDau,
                ThoiGianKetThuc = voucher.ThoiGianKetThuc,
                TrangThai = voucher.TrangThai
            };
        }
        public async Task<VoucherViewModels> GetById(Guid id)
        {
            var voucher = await _context.Voucher.
                FirstOrDefaultAsync(x => x.Id == id);
            if (voucher == null) throw new Exception($"không tìm thấy voucher có id: {id}");

            return new VoucherViewModels
            {
                Id = voucher.Id,
                MaVoucher = voucher.MaVoucher,
                LoaiGiamGia = voucher.LoaiGiamGia,
                GiaTriGiamGia = voucher.GiaTriGiamGia,
                DieuKienApDung = voucher.DieuKienApDung,
                SoLuong = voucher.SoLuong,
                NgayTao = voucher.NgayTao,
                ThoiGianBatDau = voucher.ThoiGianBatDau,
                ThoiGianKetThuc = voucher.ThoiGianKetThuc,
                TrangThai = voucher.TrangThai
            };
        }
        public async Task<VoucherViewModels> Update(UpdateVoucher request)
        {
            var voucher = await _context.Voucher.FindAsync(request.Id);
            if (voucher == null) throw new Exception("Voucher không tồn tại");

            voucher.MaVoucher = request.MaVoucher;
            voucher.LoaiGiamGia = request.LoaiGiamGia;
            voucher.GiaTriGiamGia = request.GiaTriGiamGia;
            voucher.DieuKienApDung = request.DieuKienApDung;
            voucher.SoLuong = request.SoLuong;
            voucher.ThoiGianBatDau = request.ThoiGianBatDau ;
            voucher.ThoiGianKetThuc = request.ThoiGianKetThuc ;

            _context.Voucher.Update(voucher);
            await _context.SaveChangesAsync();
            return await GetById(voucher.Id);
        }
        public async Task<bool> UpdateTrangThai(Guid Id, TrangThaiGiamGia status)
        {
            var voucher = await _context.Voucher.FindAsync(Id);
            if (voucher == null) return false;

            voucher.TrangThai = status;

            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UseVoucher(string code)
        {
            var voucher = await _context.Voucher.FirstOrDefaultAsync(x => x.MaVoucher == code);
            if (voucher == null || voucher.SoLuong <= 0 || voucher.TrangThai != TrangThaiGiamGia.HoatDong)
            {
                return false;
            }
            voucher.SoLuong--;
            // Nếu hết thì cập nhật trạng thái
            if (voucher.SoLuong == 0)
            {
                voucher.TrangThai = TrangThaiGiamGia.HetHan;
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
