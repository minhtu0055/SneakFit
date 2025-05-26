public class SPCTDetailViewModel
{
    public Guid Id { get; set; }
    public string TenSanPham { get; set; }
    public string MoTa { get; set; }
    public Guid ThuongHieuId { get; set; }
    public bool TrangThai { get; set; }
    public Guid ChatLieuId { get; set; }
    public Guid DeGiayId { get; set; }
    public string GioiTinh { get; set; }
    public Guid TheLoaiId { get; set; }
    public Guid MauSacId { get; set; }
    public Guid KichThuocId { get; set; }
    public int SoLuong { get; set; }
    public int? SoLuongHangTra { get; set; }
    public decimal GiaBan { get; set; }
    public string QRCodeUrl { get; set; }
    public List<ImageViewModel> Images { get; set; }
}

public class ImageViewModel
{
    public Guid Id { get; set; }
    public string Url { get; set; }
}