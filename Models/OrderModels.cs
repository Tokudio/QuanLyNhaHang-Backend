namespace QuanLyNhaHangAPI.Models
{
    // Class hứng dữ liệu Đơn hàng từ Angular gửi lên
    public class OrderRequestDTO
    {
        public string LoaiDonHang { get; set; } = null!;
        public string PhuongThucThanhToan { get; set; } = null!;
        public int? MaBan { get; set; }
        public string? GhiChu { get; set; }
        public List<ChiTietDonHangDTO> ChiTietDonHang { get; set; } = new List<ChiTietDonHangDTO>();
    }

    // Class hứng dữ liệu chi tiết từng món ăn
    public class ChiTietDonHangDTO
    {
        public int MaMonAn { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaLucDat { get; set; }
    }
}