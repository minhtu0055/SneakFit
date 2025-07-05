using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using System.Threading.Tasks;
using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ApiIntegration.Services.ThuongHieu;
using SneakFit.WebClient.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using SneakFit.ViewModels.Catalog.DanhMuc;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.ThuongHieu;
using SneakFit.ViewModels.Catalog.SanPham;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace SneakFit.WebClient.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ISanPhamApiClient _sanPhamApiClient;
        private readonly ISpctApiClient _spctApiClient;
        private readonly IDanhMucApiClient _danhMucApiClient;
        private readonly IMauSacApiClient _mauSacApiClient;
        private readonly IKichThuocApiClient _kichThuocApiClient;
        private readonly IThuongHieuApiClient _thuongHieuApiClient;

        public SanPhamController(
            ISanPhamApiClient sanPhamApiClient,
            ISpctApiClient spctApiClient,
            IDanhMucApiClient danhMucApiClient,
            IMauSacApiClient mauSacApiClient,
            IKichThuocApiClient kichThuocApiClient,
            IThuongHieuApiClient thuongHieuApiClient)
        {
            _sanPhamApiClient = sanPhamApiClient;
            _spctApiClient = spctApiClient;
            _danhMucApiClient = danhMucApiClient;
            _mauSacApiClient = mauSacApiClient;
            _kichThuocApiClient = kichThuocApiClient;
            _thuongHieuApiClient = thuongHieuApiClient;
        }

        // Trang Index: chỉ fill danh sách SanPham + ảnh đại diện
        public async Task<IActionResult> Index(string tuKhoa, Guid? danhMucId, decimal? giaThapNhat, decimal? giaCaoNhat, int pageIndex = 1)
        {
            var categories = await _danhMucApiClient.GetAll();
            var colors = await _mauSacApiClient.GetAll();
            var brands = await _thuongHieuApiClient.GetAll();

            var request = new SanPhamPagingRequest
            {
                Keyword = tuKhoa,
                DanhMucId = danhMucId,
                TrangThai = true,
                PageIndex = pageIndex,
                PageSize = 10
            };
            var pagedSanPham = await _sanPhamApiClient.GetAllPaging(request);
            var allSpct = new List<SPCTViewModels>();

            //tìm kiếm product
            if (!string.IsNullOrEmpty(tuKhoa))
            {
                tuKhoa = tuKhoa.ToLower().Trim();
                pagedSanPham.Items = pagedSanPham.Items
                    .Where(x => x.TenSanPham?.ToLower().Contains(tuKhoa) == true)
                    .ToList();
            }

            ViewBag.Keyword = tuKhoa;

            // Lấy 1 ảnh đại diện cho mỗi sản phẩm
            foreach (var sanPham in pagedSanPham.Items)
            {
                // Lấy danh sách SPCT theo tên sản phẩm
                var spctList = await _sanPhamApiClient.GetSPCTByProductName(sanPham.TenSanPham);
                allSpct.AddRange(spctList);
                var spctDaiDien = spctList.FirstOrDefault(spct => spct.Images != null && spct.Images.Any());
                sanPham.ImageDaiDien = spctDaiDien?.Images?.FirstOrDefault() ?? "/images/Default.jpg";
            }

            var viewModel = new DanhMucSPCTViewModel
            {
                DanhMucs = categories,
                MauSacs = colors,
                ThuongHieus = brands,
                SanPhams = pagedSanPham,
                AllSpct = allSpct,
            };

            return View(viewModel);
        }

        // Trang Details: fill full SPCT của sản phẩm để chọn màu/size
        public async Task<IActionResult> Details(Guid id)
        {
            var sanPham = await _sanPhamApiClient.GetById(id);
            var spctList = await _sanPhamApiClient.GetSPCTByProductName(sanPham.TenSanPham);
            var colors = await _mauSacApiClient.GetAll();
            var sizes = await _kichThuocApiClient.GetAll();

            var viewModel = new SanPhamDetailViewModel
            {
                SanPham = sanPham,
                SanPhamChiTiets = spctList,
                MauSacs = colors,
                KichThuocs = sizes
            };

            return View(viewModel);
        }
    }
}