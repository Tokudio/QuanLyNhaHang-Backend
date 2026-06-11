namespace QuanLyNhaHangAPI.Models
{
    // === CÁC MODEL DÀNH CHO DANH SÁCH BÀN VÀ ĐƠN MỚI ===
    public class BanAnDto
    {
        public int MaBan { get; set; }
        public string SoBan { get; set; } = null!;
        public string? TrangThaiBan { get; set; }
    }

    public class DonHangNhanVienDto
    {
        public int MaDonHang { get; set; }
        public int? MaBan { get; set; }
        public string? TenBan { get; set; }
        public string? LoaiDonHang { get; set; }
        public DateTime? NgayTao { get; set; }
        public decimal? TongTien { get; set; }
        public string? TrangThaiDon { get; set; }
    }

    public class NhanVienApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
        public T? Data { get; set; }
    }

    // === CÁC MODEL BỔ SUNG CHO CHỨC NĂNG POS ===
    public class ChiTietDonHangDto
    {
        public int MaChiTiet { get; set; }
        public int MaMonAn { get; set; }
        public string TenMonAn { get; set; } = null!;
        public int SoLuong { get; set; }
        public decimal GiaLucDat { get; set; }
        public decimal ThanhTien => SoLuong * GiaLucDat;
        public string? GhiChu { get; set; }
        public string? TrangThaiBep { get; set; }
    }

    public class DonHangHienTaiDto
    {
        public int MaDonHang { get; set; }
        public int? MaBan { get; set; }
        public string? SoBan { get; set; }
        public decimal TongTien { get; set; }
        public string? TrangThaiDon { get; set; }
        public string? TrangThaiThanhToan { get; set; }
        public List<ChiTietDonHangDto> ChiTietDonHangs { get; set; } = new List<ChiTietDonHangDto>();
    }

    public class ThemMonVaoDonRequest
    {
        public int MaDonHang { get; set; } // Nếu = 0 thì backend sẽ tự tạo đơn mới
        public int? MaBan { get; set; }    // Mã bàn (Nếu null hoặc = 0 thì hiểu là khách Mang về)
        public int MaMonAn { get; set; }
        public int SoLuong { get; set; }
    }

    public class CapNhatSoLuongRequest
    {
        public int SoLuongMoi { get; set; }
    }
}