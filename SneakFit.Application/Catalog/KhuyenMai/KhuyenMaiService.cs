using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.Data.Entities;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.KhuyenMai
{
    public class KhuyenMaiService : IKhuyenMaiService
    {
        private readonly SneakFitDbContext _context;

        public KhuyenMaiService(SneakFitDbContext context)
        {
            _context = context;
        }

        // Phương thức tạo mới khuyến mãi
        public async Task<KhuyenMaiViewModels> Create(ThemKhuyenMai request)
        {
            // Kiểm tra nếu danh sách sản phẩm trống
            if (request.SanPhamIds == null || !request.SanPhamIds.Any())
            {
                throw new Exception("Danh sách sản phẩm không thể trống.");
            }

            // Tạo đối tượng Khuyến Mại mới
            var km = new SneakFit.Data.Entities.KhuyenMai()
            {
                Id = Guid.NewGuid(),
                TenKhuyenMai = request.TenKhuyenMai,
                MoTa = request.MoTa,
                NgayTao = DateTime.Now,
                ThoiGianBatDau = request.ThoiGianBatDau,
                ThoiGianKetThuc = request.ThoiGianKetThuc,
                LoaiGiamGia = request.LoaiGiamGia,
                GiaTriGiamGia = request.GiaTriGiamGia,
                TrangThai = request.TrangThai,
                KhuyenMaiChiTiet = new List<KhuyenMaiChiTiet>()
            };

            // Kiểm tra tính hợp lệ của các sản phẩm trong danh sách
            foreach (var sanPhamId in request.SanPhamIds)
            {
                var sanPham = await _context.SanPham.FirstOrDefaultAsync(sp => sp.Id == sanPhamId);
                if (sanPham == null)
                {
                    throw new Exception($"Sản phẩm với ID {sanPhamId} không tồn tại.");
                }

                // Thêm chi tiết khuyến mãi vào danh sách
                km.KhuyenMaiChiTiet.Add(new KhuyenMaiChiTiet
                {
                    SanPhamId = sanPhamId
                });
            }

            // Lưu khuyến mãi vào cơ sở dữ liệu
            _context.KhuyenMai.Add(km);
            await _context.SaveChangesAsync();

            // Trả về thông tin khuyến mãi mới tạo
            return await GetById(km.Id);
        }

        // Phương thức lấy tất cả các khuyến mãi
        public async Task<List<KhuyenMaiViewModels>> GetAll()
        {
            return await _context.KhuyenMai
                .Include(x => x.KhuyenMaiChiTiet)
                .Select(km => new KhuyenMaiViewModels
                {
                    Id = km.Id,
                    TenKhuyenMai = km.TenKhuyenMai,
                    MoTa = km.MoTa,
                    ThoiGianBatDau = km.ThoiGianBatDau,
                    ThoiGianKetThuc = km.ThoiGianKetThuc,
                    LoaiGiamGia = km.LoaiGiamGia,
                    GiaTriGiamGia = km.GiaTriGiamGia,
                    TrangThai = km.TrangThai,
                    SanPhamIds = km.KhuyenMaiChiTiet
                    .Where(ct => ct.SanPhamId.HasValue) // Kiểm tra giá trị không phải null
                    .Select(ct => ct.SanPhamId.Value)   // Lấy giá trị Guid thực tế
                    .ToList()

                })
                .ToListAsync();
        }

        // Phương thức lấy thông tin khuyến mãi theo ID
        public async Task<KhuyenMaiViewModels> GetById(Guid id)
        {
            var km = await _context.KhuyenMai
                .Include(x => x.KhuyenMaiChiTiet)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (km == null) return null;

            return new KhuyenMaiViewModels
            {
                Id = km.Id,
                TenKhuyenMai = km.TenKhuyenMai,
                MoTa = km.MoTa,
                ThoiGianBatDau = km.ThoiGianBatDau,
                ThoiGianKetThuc = km.ThoiGianKetThuc,
                LoaiGiamGia = km.LoaiGiamGia,
                GiaTriGiamGia = km.GiaTriGiamGia,
                TrangThai = km.TrangThai,
                SanPhamIds = km.KhuyenMaiChiTiet
                .Where(ct => ct.SanPhamId.HasValue) // Kiểm tra giá trị không phải null
                .Select(ct => ct.SanPhamId.Value)   // Lấy giá trị Guid thực tế
                .ToList()

            };
        }

        // Phương thức cập nhật khuyến mãi
        public async Task<KhuyenMaiViewModels> Update(SuaKhuyenMai request)
        {
            var km = await _context.KhuyenMai
                .Include(x => x.KhuyenMaiChiTiet)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (km == null) return null;

            // Cập nhật thông tin khuyến mãi
            km.TenKhuyenMai = request.TenKhuyenMai;
            km.MoTa = request.MoTa;
            km.ThoiGianBatDau = request.ThoiGianBatDau;
            km.ThoiGianKetThuc = request.ThoiGianKetThuc;
            km.LoaiGiamGia = request.LoaiGiamGia;
            km.GiaTriGiamGia = request.GiaTriGiamGia;
            km.TrangThai = request.TrangThai;

            // Cập nhật lại chi tiết khuyến mãi
            _context.KhuyenMaiChiTiet.RemoveRange(km.KhuyenMaiChiTiet); // Xóa chi tiết khuyến mãi cũ
            km.KhuyenMaiChiTiet = new List<KhuyenMaiChiTiet>(); // Khởi tạo lại danh sách chi tiết

            // Kiểm tra và thêm lại các chi tiết khuyến mãi mới
            foreach (var sanPhamId in request.SanPhamIds)
            {
                var sanPham = await _context.SanPham.FirstOrDefaultAsync(sp => sp.Id == sanPhamId);
                if (sanPham == null)
                {
                    throw new Exception($"Sản phẩm với ID {sanPhamId} không tồn tại.");
                }

                km.KhuyenMaiChiTiet.Add(new KhuyenMaiChiTiet
                {
                    KhuyenMaiId = km.Id,
                    SanPhamId = sanPhamId
                });
            }

            await _context.SaveChangesAsync();

            return await GetById(km.Id);
        }
    }
}
