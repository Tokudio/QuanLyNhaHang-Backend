using QuanLyNhaHangAPI.Models;

namespace QuanLyNhaHangAPI.Services
{
    public interface INhanVienService
    {
        // Chức năng cơ bản
        Task<NhanVienApiResponse<List<BanAnDto>>> LayDanhSachBanAnAsync();
        Task<NhanVienApiResponse<List<DonHangNhanVienDto>>> LayDonHangMoiAsync();

        // Chức năng POS
        Task<NhanVienApiResponse<DonHangHienTaiDto>> LayDonHangHienTaiTheoBanAsync(int maBan);
        Task<NhanVienApiResponse<bool>> CapNhatSoLuongMonAsync(int maChiTiet, int soLuongMoi);

        // CHÚ Ý SỬA Ở ĐÂY: Trả về int thay vì bool
        Task<NhanVienApiResponse<int>> ThemMonVaoDonHienTaiAsync(ThemMonVaoDonRequest request);

        Task<NhanVienApiResponse<bool>> XacNhanThanhToanAsync(int maDonHang);
    }
}