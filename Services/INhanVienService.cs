using QuanLyNhaHangAPI.Models;

namespace QuanLyNhaHangAPI.Services
{
    public interface INhanVienService
    {
        // Chức năng đã có
        Task<NhanVienApiResponse<List<BanAnDto>>> LayDanhSachBanAnAsync();
        Task<NhanVienApiResponse<List<DonHangNhanVienDto>>> LayDonHangMoiAsync();

        // Chức năng POS mới
        Task<NhanVienApiResponse<DonHangHienTaiDto>> LayDonHangHienTaiTheoBanAsync(int maBan);
        Task<NhanVienApiResponse<bool>> CapNhatSoLuongMonAsync(int maChiTiet, int soLuongMoi);
        Task<NhanVienApiResponse<bool>> ThemMonVaoDonHienTaiAsync(ThemMonVaoDonRequest request);
        Task<NhanVienApiResponse<bool>> XacNhanThanhToanAsync(int maDonHang);
    }
}