using Microsoft.AspNetCore.Mvc;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;

namespace SneakFit.Admin.Controllers
{
    public class HoaDonController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        private List<TrangThaiStep> GetTrangThaiSteps(TrangThaiHoaDon trangThaiHienTai, Dictionary<TrangThaiHoaDon, string> thoiGianTungBuoc = null)
        {
            var steps = new List<TrangThaiStep>
            {
                new TrangThaiStep
                {
                    Label = "Chờ xác nhận",
                    Icon = "bx-time",
                    Time = thoiGianTungBuoc != null && thoiGianTungBuoc.ContainsKey(TrangThaiHoaDon.ChoXacNhan) ? thoiGianTungBuoc[TrangThaiHoaDon.ChoXacNhan] : "",
                    IsActive = trangThaiHienTai >= TrangThaiHoaDon.ChoXacNhan
                },
                new TrangThaiStep
                {
                    Label = "Đã xác nhận",
                    Icon = "bx-check-circle",
                    Time = thoiGianTungBuoc != null && thoiGianTungBuoc.ContainsKey(TrangThaiHoaDon.DaXacNhan) ? thoiGianTungBuoc[TrangThaiHoaDon.DaXacNhan] : "",
                    IsActive = trangThaiHienTai >= TrangThaiHoaDon.DaXacNhan
                },
                new TrangThaiStep
                {
                    Label = "Chờ vận chuyển",
                    Icon = "bx-package",
                    Time = thoiGianTungBuoc != null && thoiGianTungBuoc.ContainsKey(TrangThaiHoaDon.ChoVanChuyen) ? thoiGianTungBuoc[TrangThaiHoaDon.ChoVanChuyen] : "",
                    IsActive = trangThaiHienTai >= TrangThaiHoaDon.ChoVanChuyen
                },
                new TrangThaiStep
                {
                    Label = "Đang vận chuyển",
                    Icon = "bx-car",
                    Time = thoiGianTungBuoc != null && thoiGianTungBuoc.ContainsKey(TrangThaiHoaDon.DangVanChuyen) ? thoiGianTungBuoc[TrangThaiHoaDon.DangVanChuyen] : "",
                    IsActive = trangThaiHienTai >= TrangThaiHoaDon.DangVanChuyen
                },
                new TrangThaiStep
                {
                    Label = "Đã thanh toán",
                    Icon = "bx-credit-card",
                    Time = thoiGianTungBuoc != null && thoiGianTungBuoc.ContainsKey(TrangThaiHoaDon.DaThanhToan) ? thoiGianTungBuoc[TrangThaiHoaDon.DaThanhToan] : "",
                    IsActive = trangThaiHienTai >= TrangThaiHoaDon.DaThanhToan
                },
                new TrangThaiStep
                {
                    Label = "Hoàn thành",
                    Icon = "bx-check-double",
                    Time = thoiGianTungBuoc != null && thoiGianTungBuoc.ContainsKey(TrangThaiHoaDon.HoanThanh) ? thoiGianTungBuoc[TrangThaiHoaDon.HoanThanh] : "",
                    IsActive = trangThaiHienTai >= TrangThaiHoaDon.HoanThanh
                },
                new TrangThaiStep
                {
                    Label = "Đã hủy",
                    Icon = "bx-x",
                    Time = thoiGianTungBuoc != null && thoiGianTungBuoc.ContainsKey(TrangThaiHoaDon.DaHuy) ? thoiGianTungBuoc[TrangThaiHoaDon.DaHuy] : "",
                    IsActive = trangThaiHienTai == TrangThaiHoaDon.DaHuy
                }
            };
            if (trangThaiHienTai == TrangThaiHoaDon.DaHuy)
            {
                foreach (var step in steps)
                    step.IsActive = step.Label == "Đã hủy";
            }
            return steps;
        }

        public IActionResult Details(Guid id)
        {
            var trangThaiHienTai = TrangThaiHoaDon.HoanThanh;
            var thoiGianTungBuoc = new Dictionary<TrangThaiHoaDon, string>
            {
                { TrangThaiHoaDon.ChoXacNhan, "16:37:35 23-12-2023" },
                { TrangThaiHoaDon.DaXacNhan, "16:39:00 23-12-2023" },
                { TrangThaiHoaDon.ChoVanChuyen, "16:39:47 23-12-2023" },
                { TrangThaiHoaDon.DangVanChuyen, "16:40:19 23-12-2023" },
                { TrangThaiHoaDon.DaThanhToan, "16:40:22 23-12-2023" },
                { TrangThaiHoaDon.HoanThanh, "16:40:26 23-12-2023" },
                { TrangThaiHoaDon.DaHuy, "" }
            };

            var model = new HoaDonChiTietViewModels
            {
                MaHoaDon = "HD881712",
                TrangThai = "Đang vận chuyển",
                LoaiDon = "Online",
                DiaChi = "10 Trung Dương Thanh, Xã Yên Thịnh, Huyện Chợ Đồn, Bắc Kạn",
                GhiChu = "Giao giờ hành chính",
                TenKhachHang = "Vinh Nguyen Van",
                SoDienThoai = "0378500000",
                ThoiGianDuKienNhan = DateTime.Now.AddDays(2),
                TrangThaiSteps = GetTrangThaiSteps(trangThaiHienTai, thoiGianTungBuoc),
                LichSuThanhToan = new List<LichSuThanhToanViewModels>
                {
                    new LichSuThanhToanViewModels
                    {
                        MaGiaoDich = "GD001",
                        SoTien = 9350000,
                        TrangThai = "Tiền mặt",
                        ThoiGian = DateTime.Parse("2023-12-23 16:40"),
                        LoaiGiaoDich = "Thanh toán",
                        PhuongThucThanhToan = "Tiền mặt",
                        GhiChu = "",
                        NguoiXacNhan = "Nguyễn Văn Vinh"
                    }
                },
                SanPhamMua = new List<SanPhamMuaViewModels>
                {
                    new SanPhamMuaViewModels
                    {
                        AnhSanPham = "",
                        TenSanPham = "Giày Thể Thao Nam Nike Dbreak-Type",
                        GiaBan = 2500000,
                        KichCo = "40",
                        MaMau = "#000000",
                        TenMau = "Đen",
                        SoLuong = 1,
                        ThanhTien = 2500000,
                        TrangThai = "Thành công"
                    },
                    new SanPhamMuaViewModels
                    {
                        AnhSanPham = "",
                        TenSanPham = "Giày Chạy Nam Adidas Ultraboost",
                        GiaBan = 2500000,
                        KichCo = "42",
                        MaMau = "#000000",
                        TenMau = "Đen",
                        SoLuong = 1,
                        ThanhTien = 2500000,
                        TrangThai = "Thành công"
                    },
                    new SanPhamMuaViewModels
                    {
                        AnhSanPham = "",
                        TenSanPham = "KAPPA GIÀY SNEAKERS 123",
                        GiaBan = 4500000,
                        KichCo = "36",
                        MaMau = "#FF0000",
                        TenMau = "Đỏ",
                        SoLuong = 1,
                        ThanhTien = 4500000,
                        TrangThai = "Thành công"
                    }
                },
                TongTienHang = 9500000,
                PhiVanChuyen = 0,
                VoucherGiamGia = 150000,
                TongTienGiam = 150000,
                TongTienThanhToan = 9350000
            };
            return View(model);
        }
    }
}
