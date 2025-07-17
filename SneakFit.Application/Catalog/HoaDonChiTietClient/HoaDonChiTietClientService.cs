using Microsoft.EntityFrameworkCore;
using SneakFit.Application.Catalog.HoaDonChiTietClients;
using SneakFit.Data.EF;
using SneakFit.Data.Entities;
using SneakFit.ViewModels.Catalog.HoaDonChiTietClient;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.HoaDonChiTietClientClient
{
    public class HoaDonChiTietClientService : IHoaDonChiTietClientService
    {
        private readonly SneakFitDbContext _context;

        public HoaDonChiTietClientService(SneakFitDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<HoaDonChiTietClientViewModel>> GetAllPaging(PhanTrangHoaDonChiTietClient request)
        {
            var query = _context.HoaDonChiTiet
                .Include(h => h.HoaDon)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.SanPham)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.MauSac)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.KichThuoc)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.HinhAnhSanPham)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.HoaDon.MaHoaDon.Contains(request.Keyword));
            }

            int totalRow = await query.CountAsync();
            var data = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new HoaDonChiTietClientViewModel()
                {
                    Id = x.Id,
                    SoLuong = x.SoLuong,
                    GiaBan = x.GiaBan,
                    TenSanPham = x.SanPhamChiTiet.SanPham.TenSanPham ?? "Không xác định",
                    TenMauSac = x.SanPhamChiTiet.MauSac.TenMauSac ?? "Không xác định",
                    MaKichThuoc = x.SanPhamChiTiet.KichThuoc.MaKichThuoc.ToString() ?? "Không xác định",
                    AnhSanPham = x.SanPhamChiTiet.HinhAnhSanPham.FirstOrDefault().UrlHinhAnh ?? "/images/Default_Logo.png"
                }).ToListAsync();
            var pagedResult = new PagedResult<HoaDonChiTietClientViewModel>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<List<HoaDonChiTietClientViewModel>> GetById(Guid id)
        {
            var chiTiets = await _context.HoaDonChiTiet
                .Where(x => x.HoaDonId == id)
                .Include(h => h.HoaDon)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.SanPham)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.MauSac)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.KichThuoc)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.HinhAnhSanPham)
                .ToListAsync();

            return chiTiets.Select(hoaDonChiTietClient => {
                // Lấy khuyến mãi cho từng SPCT
                var kmct = _context.KhuyenMaiChiTiet
                    .Include(x => x.KhuyenMai)
                    .FirstOrDefault(x => x.SPCTId == hoaDonChiTietClient.SanPhamChiTietId && x.KhuyenMai.ThoiGianBatDau <= DateTime.Now && x.KhuyenMai.ThoiGianKetThuc >= DateTime.Now && x.KhuyenMai.TrangThai == SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong);
                decimal? giaKhuyenMai = null;
                decimal? khuyenMaiPhanTram = null;
                Guid? khuyenMaiId = null;
                string tenKhuyenMai = null;
                if (kmct != null && kmct.KhuyenMai != null)
                {
                    khuyenMaiId = kmct.KhuyenMaiId;
                    tenKhuyenMai = kmct.KhuyenMai.TenKhuyenMai;
                    if (kmct.KhuyenMai.LoaiGiamGia == SneakFit.Data.Enums.LoaiGiamGia.PhamTram)
                    {
                        khuyenMaiPhanTram = kmct.KhuyenMai.GiaTriGiamGia;
                        giaKhuyenMai = Math.Round(hoaDonChiTietClient.GiaBan * (1 - kmct.KhuyenMai.GiaTriGiamGia / 100), 0);
                    }
                    else
                    {
                        khuyenMaiPhanTram = null;
                        giaKhuyenMai = Math.Max(0, hoaDonChiTietClient.GiaBan - kmct.KhuyenMai.GiaTriGiamGia);
                    }
                }
                return new HoaDonChiTietClientViewModel()
                {
                    Id = hoaDonChiTietClient.Id,
                    SoLuong = hoaDonChiTietClient.SoLuong,
                    GiaBan = hoaDonChiTietClient.GiaBan,
                    TenSanPham = hoaDonChiTietClient.SanPhamChiTiet?.SanPham?.TenSanPham ?? "Không xác định",
                    TenMauSac = hoaDonChiTietClient.SanPhamChiTiet?.MauSac?.TenMauSac ?? "Không xác định",
                    MaKichThuoc = hoaDonChiTietClient.SanPhamChiTiet?.KichThuoc?.MaKichThuoc.ToString() ?? "Không xác định",
                    AnhSanPham = hoaDonChiTietClient.SanPhamChiTiet?.HinhAnhSanPham?.FirstOrDefault()?.UrlHinhAnh ?? "/images/Default_Logo.png",
                    GiaKhuyenMai = giaKhuyenMai,
                    KhuyenMaiPhanTram = khuyenMaiPhanTram,
                    KhuyenMaiId = khuyenMaiId,
                    TenKhuyenMai = tenKhuyenMai
                };
            }).ToList();
        }

        public async Task<HoaDonChiTietClientViewModel> Create(ThemHoaDonChiTietClient request)
        {
            var hoaDonChiTietClient = new Data.Entities.HoaDonChiTiet()
            {
                Id = Guid.NewGuid(),
                SoLuong = request.SoLuong,
                GiaBan = request.GiaBan,
                HoaDonId = request.HoaDonId,
                SanPhamChiTietId = request.SanPhamChiTietId
            };
            _context.HoaDonChiTiet.Add(hoaDonChiTietClient);
            await _context.SaveChangesAsync();

            // Tải lại dữ liệu để đảm bảo mối quan hệ được điền
            var createdEntity = await _context.HoaDonChiTiet
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.SanPham)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.MauSac)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.KichThuoc)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.HinhAnhSanPham)
                .FirstOrDefaultAsync(h => h.Id == hoaDonChiTietClient.Id);

            return new HoaDonChiTietClientViewModel
            {
                Id = createdEntity.Id,
                SoLuong = createdEntity.SoLuong,
                GiaBan = createdEntity.GiaBan,
                TenSanPham = createdEntity.SanPhamChiTiet?.SanPham?.TenSanPham ?? "Không xác định",
                TenMauSac = createdEntity.SanPhamChiTiet?.MauSac?.TenMauSac ?? "Không xác định",
                MaKichThuoc = createdEntity.SanPhamChiTiet?.KichThuoc?.MaKichThuoc.ToString() ?? "Không xác định",
                AnhSanPham = hoaDonChiTietClient.SanPhamChiTiet?.HinhAnhSanPham?.FirstOrDefault()?.UrlHinhAnh ?? "/images/Default_Logo.png"
            };
        }

        public async Task<HoaDonChiTietClientViewModel> Edit(SuaHoaDonChiTietClient request)
        {
            var hoaDonChiTietClient = await _context.HoaDonChiTiet
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.SanPham)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.MauSac)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.KichThuoc)
                .Include(h => h.SanPhamChiTiet)
                    .ThenInclude(spct => spct.HinhAnhSanPham)
                .FirstOrDefaultAsync(h => h.Id == request.Id);

            if (hoaDonChiTietClient == null) return null;

            hoaDonChiTietClient.SoLuong = request.SoLuong;
            hoaDonChiTietClient.GiaBan = request.GiaBan;
            await _context.SaveChangesAsync();

            return new HoaDonChiTietClientViewModel
            {
                Id = hoaDonChiTietClient.Id,
                SoLuong = hoaDonChiTietClient.SoLuong,
                GiaBan = hoaDonChiTietClient.GiaBan,
                TenSanPham = hoaDonChiTietClient.SanPhamChiTiet?.SanPham?.TenSanPham ?? "Không xác định",
                TenMauSac = hoaDonChiTietClient.SanPhamChiTiet?.MauSac?.TenMauSac ?? "Không xác định",
                MaKichThuoc = hoaDonChiTietClient.SanPhamChiTiet?.KichThuoc?.MaKichThuoc.ToString() ?? "Không xác định",
                AnhSanPham = hoaDonChiTietClient.SanPhamChiTiet?.HinhAnhSanPham?.FirstOrDefault()?.UrlHinhAnh ?? "/images/Default_Logo.png"
            };
        }
    }
}