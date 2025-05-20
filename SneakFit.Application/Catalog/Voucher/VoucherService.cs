using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.Data.Entities;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.Voucher;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.Voucher
{
    public class VoucherService : IVoucherService
    {
        public readonly SneakFitDbContext _context;
        public VoucherService(SneakFitDbContext context)
        {
            _context = context;
        }

        // Hàm kiểm tra và cập nhật trạng thái voucher
        private TrangThaiGiamGia GetVoucherStatus(DateTime startTime, DateTime endTime)
        {
            var now = DateTime.Now;
            if (now < startTime)
                return TrangThaiGiamGia.KhongHoatDong;
            if (now > endTime)
                return TrangThaiGiamGia.HetHan;
            return TrangThaiGiamGia.HoatDong;
        }

        public async Task<VoucherViewModels> Create(CreateVoucher request)
        {
            // Kiểm tra mã voucher trùng lặp
            var existingVoucher = await _context.Voucher.FirstOrDefaultAsync(x => x.MaVoucher == request.MaVoucher);
            if (existingVoucher != null)
            {
                throw new Exception($"Mã voucher '{request.MaVoucher}' đã tồn tại trong hệ thống.");
            }

            var vc = new SneakFit.Data.Entities.Voucher()
            {
                Id = Guid.NewGuid(),
                MaVoucher = request.MaVoucher,
                LoaiGiamGia = request.LoaiGiamGia,
                GiaTriGiamGia = request.GiaTriGiamGia,
                DieuKienApDung = request.DieuKienApDung,
                SoLuong = request.SoLuong,
                NgayTao = DateTime.Now,
                ThoiGianBatDau = request.ThoiGianBatDau,
                ThoiGianKetThuc = request.ThoiGianKetThuc,
                TrangThai = GetVoucherStatus(request.ThoiGianBatDau, request.ThoiGianKetThuc)
            };
            _context.Voucher.Add(vc);
            await _context.SaveChangesAsync();
            return await GetById(vc.Id);
        }

        public async Task<PagedResult<VoucherViewModels>> GetAllPaging(GetVoucherPagingRequest request)
        {
            var query = _context.Voucher.AsQueryable();

            // Cập nhật trạng thái cho tất cả voucher trước khi lấy danh sách
            var vouchers = await query.ToListAsync();
            foreach (var voucher in vouchers)
            {
                var newStatus = GetVoucherStatus(voucher.ThoiGianBatDau, voucher.ThoiGianKetThuc);
                if (voucher.TrangThai != newStatus)
                {
                    voucher.TrangThai = newStatus;
                }
            }
            await _context.SaveChangesAsync();

            // Lọc theo từ khóa
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.MaVoucher.Contains(request.Keyword));
            }

            // Lọc theo trạng thái
            if (request.Status.HasValue)
            {
                query = query.Where(x => x.TrangThai == request.Status.Value);
            }

            // Tính toán tổng số bản ghi
            int totalRow = await query.CountAsync();

            // Lấy dữ liệu theo trang
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
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

            // Tạo đối tượng phân trang
            var pagedResult = new PagedResult<VoucherViewModels>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };

            return pagedResult;
        }

        public async Task<VoucherViewModels> GetById(Guid id)
        {
            var voucher = await _context.Voucher.FindAsync(id);
            if (voucher == null) return null;

            // Cập nhật trạng thái trước khi trả về
            var newStatus = GetVoucherStatus(voucher.ThoiGianBatDau, voucher.ThoiGianKetThuc);
            if (voucher.TrangThai != newStatus)
            {
                voucher.TrangThai = newStatus;
                await _context.SaveChangesAsync();
            }

            return new VoucherViewModels()
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

        public async Task<VoucherViewModels> GetByCode(string code)
        {
            var voucher = await _context.Voucher.FirstOrDefaultAsync(x => x.MaVoucher == code);
            if (voucher == null) return null;

            // Cập nhật trạng thái trước khi trả về
            var newStatus = GetVoucherStatus(voucher.ThoiGianBatDau, voucher.ThoiGianKetThuc);
            if (voucher.TrangThai != newStatus)
            {
                voucher.TrangThai = newStatus;
                await _context.SaveChangesAsync();
            }

            return new VoucherViewModels()
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
            if (voucher == null) return null;

            // Kiểm tra mã voucher trùng lặp (trừ voucher hiện tại)
            var existingVoucher = await _context.Voucher.FirstOrDefaultAsync(x => x.MaVoucher == request.MaVoucher && x.Id != request.Id);
            if (existingVoucher != null)
            {
                throw new Exception($"Mã voucher '{request.MaVoucher}' đã tồn tại trong hệ thống.");
            }

            voucher.MaVoucher = request.MaVoucher;
            voucher.LoaiGiamGia = request.LoaiGiamGia;
            voucher.GiaTriGiamGia = request.GiaTriGiamGia;
            voucher.DieuKienApDung = request.DieuKienApDung;
            voucher.SoLuong = request.SoLuong;
            voucher.ThoiGianBatDau = request.ThoiGianBatDau;
            voucher.ThoiGianKetThuc = request.ThoiGianKetThuc;
            
            // Cập nhật trạng thái dựa trên thời gian
            voucher.TrangThai = GetVoucherStatus(request.ThoiGianBatDau, request.ThoiGianKetThuc);

            _context.Voucher.Update(voucher);
            await _context.SaveChangesAsync();
            return await GetById(voucher.Id);
        }

        public async Task<bool> UpdateTrangThai(Guid Id, TrangThaiGiamGia status)
        {
            var voucher = await _context.Voucher.FindAsync(Id);
            if (voucher == null) return false;

            // Chỉ cho phép cập nhật trạng thái thủ công nếu voucher chưa hết hạn
            if (DateTime.Now <= voucher.ThoiGianKetThuc)
            {
                voucher.TrangThai = status;
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UseVoucher(string code)
        {
            var voucher = await _context.Voucher.FirstOrDefaultAsync(x => x.MaVoucher == code);
            if (voucher == null) return false;

            // Cập nhật trạng thái trước khi kiểm tra
            var newStatus = GetVoucherStatus(voucher.ThoiGianBatDau, voucher.ThoiGianKetThuc);
            if (voucher.TrangThai != newStatus)
            {
                voucher.TrangThai = newStatus;
                await _context.SaveChangesAsync();
            }

            // Kiểm tra điều kiện sử dụng voucher
            if (voucher.SoLuong <= 0 || voucher.TrangThai != TrangThaiGiamGia.HoatDong)
            {
                return false;
            }

            voucher.SoLuong--;
            // Nếu hết số lượng thì cập nhật trạng thái
            if (voucher.SoLuong == 0)
            {
                voucher.TrangThai = TrangThaiGiamGia.HetHan;
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
