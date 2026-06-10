using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAPI.Data;
using QuanLyNhaHangAPI.Data.Entities;
using QuanLyNhaHangAPI.Models;

namespace QuanLyNhaHangAPI.Services
{
    public class NhanVienService : INhanVienService
    {
        private readonly QuanLyNhaHangDbContext _context;

        public NhanVienService(QuanLyNhaHangDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. CÁC HÀM CŨ ĐÃ CÓ (Giữ nguyên cấu trúc của bạn)
        // ==========================================
        public async Task<NhanVienApiResponse<List<BanAnDto>>> LayDanhSachBanAnAsync()
        {
            try
            {
                var danhSachBan = await _context.BanAn
                    .AsNoTracking()
                    .Select(b => new BanAnDto
                    {
                        MaBan = b.MaBan,
                        SoBan = b.SoBan,
                        TrangThaiBan = b.TrangThaiBan
                    })
                    .ToListAsync();

                return new NhanVienApiResponse<List<BanAnDto>> { IsSuccess = true, Message = "Lấy dữ liệu bàn ăn thành công.", Data = danhSachBan };
            }
            catch (Exception ex)
            {
                return new NhanVienApiResponse<List<BanAnDto>> { IsSuccess = false, Message = $"Lỗi khi tải sơ đồ bàn: {ex.Message}" };
            }
        }

        public async Task<NhanVienApiResponse<List<DonHangNhanVienDto>>> LayDonHangMoiAsync()
        {
            try
            {
                var danhSachDonHang = await _context.DonHang
                    .Include(d => d.MaBanNavigation)
                    .Where(d => d.TrangThaiDon == "Chờ xác nhận" || d.TrangThaiDon == "Đang chuẩn bị")
                    .OrderByDescending(d => d.NgayTao)
                    .Select(d => new DonHangNhanVienDto
                    {
                        MaDonHang = d.MaDonHang,
                        MaBan = d.MaBan,
                        TenBan = d.MaBanNavigation != null ? d.MaBanNavigation.SoBan : "Mua mang về",
                        LoaiDonHang = d.LoaiDonHang,
                        NgayTao = d.NgayTao,
                        TongTien = d.TongTien,
                        TrangThaiDon = d.TrangThaiDon
                    })
                    .ToListAsync();

                return new NhanVienApiResponse<List<DonHangNhanVienDto>> { IsSuccess = true, Message = "Lấy danh sách đơn hàng thành công.", Data = danhSachDonHang };
            }
            catch (Exception ex)
            {
                return new NhanVienApiResponse<List<DonHangNhanVienDto>> { IsSuccess = false, Message = $"Lỗi khi tải đơn hàng: {ex.Message}" };
            }
        }

        // ==========================================
        // 2. CÁC HÀM XỬ LÝ NGHIỆP VỤ POS MỚI
        // ==========================================

        public async Task<NhanVienApiResponse<DonHangHienTaiDto>> LayDonHangHienTaiTheoBanAsync(int maBan)
        {
            try
            {
                var donHang = await _context.DonHang
                    .Include(d => d.MaBanNavigation)
                    .Include(d => d.ChiTietDonHang)
                        .ThenInclude(c => c.MaMonAnNavigation)
                    .Where(d => d.MaBan == maBan && (d.TrangThaiThanhToan == "Chưa thanh toán" || d.TrangThaiThanhToan == null))
                    .OrderByDescending(d => d.NgayTao)
                    .FirstOrDefaultAsync();

                if (donHang == null)
                    return new NhanVienApiResponse<DonHangHienTaiDto> { IsSuccess = false, Message = "Bàn này hiện chưa có hóa đơn đang xử lý." };

                var donHangDto = new DonHangHienTaiDto
                {
                    MaDonHang = donHang.MaDonHang,
                    MaBan = donHang.MaBan,
                    SoBan = donHang.MaBanNavigation?.SoBan,
                    TongTien = donHang.TongTien ?? 0,
                    TrangThaiDon = donHang.TrangThaiDon,
                    TrangThaiThanhToan = donHang.TrangThaiThanhToan,
                    ChiTietDonHangs = donHang.ChiTietDonHang.Select(c => new ChiTietDonHangDto
                    {
                        MaChiTiet = c.MaChiTiet,
                        MaMonAn = c.MaMonAn,
                        TenMonAn = c.MaMonAnNavigation.TenMonAn,
                        SoLuong = c.SoLuong,
                        GiaLucDat = c.GiaLucDat,
                        GhiChu = c.GhiChu,
                        TrangThaiBep = c.TrangThaiBep
                    }).ToList()
                };

                return new NhanVienApiResponse<DonHangHienTaiDto> { IsSuccess = true, Message = "Thành công", Data = donHangDto };
            }
            catch (Exception ex)
            {
                return new NhanVienApiResponse<DonHangHienTaiDto> { IsSuccess = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<NhanVienApiResponse<bool>> CapNhatSoLuongMonAsync(int maChiTiet, int soLuongMoi)
        {
            try
            {
                var chiTiet = await _context.ChiTietDonHang.FirstOrDefaultAsync(c => c.MaChiTiet == maChiTiet);
                if (chiTiet == null)
                    return new NhanVienApiResponse<bool> { IsSuccess = false, Message = "Không tìm thấy chi tiết món ăn." };

                if (soLuongMoi <= 0)
                {
                    _context.ChiTietDonHang.Remove(chiTiet);
                }
                else
                {
                    chiTiet.SoLuong = soLuongMoi;
                }

                await _context.SaveChangesAsync();
                await TinhLaiTongTienDonHangAsync(chiTiet.MaDonHang);

                return new NhanVienApiResponse<bool> { IsSuccess = true, Message = "Cập nhật số lượng thành công.", Data = true };
            }
            catch (Exception ex)
            {
                return new NhanVienApiResponse<bool> { IsSuccess = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<NhanVienApiResponse<bool>> ThemMonVaoDonHienTaiAsync(ThemMonVaoDonRequest request)
        {
            try
            {
                var monAn = await _context.MonAn.FindAsync(request.MaMonAn);
                if (monAn == null) return new NhanVienApiResponse<bool> { IsSuccess = false, Message = "Món ăn không tồn tại." };

                var chiTietTonTai = await _context.ChiTietDonHang
                    .FirstOrDefaultAsync(c => c.MaDonHang == request.MaDonHang && c.MaMonAn == request.MaMonAn);

                if (chiTietTonTai != null)
                {
                    chiTietTonTai.SoLuong += request.SoLuong;
                }
                else
                {
                    var chiTietMoi = new ChiTietDonHang
                    {
                        MaDonHang = request.MaDonHang,
                        MaMonAn = request.MaMonAn,
                        SoLuong = request.SoLuong,
                        GiaLucDat = monAn.Gia,
                        TrangThaiBep = "Chờ chế biến",
                        NgayTao = DateTime.Now
                    };
                    await _context.ChiTietDonHang.AddAsync(chiTietMoi);
                }

                await _context.SaveChangesAsync();
                await TinhLaiTongTienDonHangAsync(request.MaDonHang);

                return new NhanVienApiResponse<bool> { IsSuccess = true, Message = "Thêm món thành công.", Data = true };
            }
            catch (Exception ex)
            {
                return new NhanVienApiResponse<bool> { IsSuccess = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<NhanVienApiResponse<bool>> XacNhanThanhToanAsync(int maDonHang)
        {
            try
            {
                var donHang = await _context.DonHang.FindAsync(maDonHang);
                if (donHang == null) return new NhanVienApiResponse<bool> { IsSuccess = false, Message = "Đơn hàng không tồn tại." };

                // Cập nhật trạng thái đơn
                donHang.TrangThaiThanhToan = "Đã thanh toán";
                donHang.TrangThaiDon = "Hoàn thành";

                // Giải phóng bàn ăn
                if (donHang.MaBan.HasValue)
                {
                    var banAn = await _context.BanAn.FindAsync(donHang.MaBan.Value);
                    if (banAn != null)
                    {
                        banAn.TrangThaiBan = "Trống";
                    }
                }

                await _context.SaveChangesAsync();
                return new NhanVienApiResponse<bool> { IsSuccess = true, Message = "Thanh toán thành công. Đã làm trống bàn.", Data = true };
            }
            catch (Exception ex)
            {
                return new NhanVienApiResponse<bool> { IsSuccess = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        // --- Hàm Helper hỗ trợ tính toán nội bộ ---
        private async Task TinhLaiTongTienDonHangAsync(int maDonHang)
        {
            var donHang = await _context.DonHang
                .Include(d => d.ChiTietDonHang)
                .FirstOrDefaultAsync(d => d.MaDonHang == maDonHang);

            if (donHang != null)
            {
                donHang.TongTien = donHang.ChiTietDonHang.Sum(c => c.SoLuong * c.GiaLucDat);
                await _context.SaveChangesAsync();
            }
        }
    }
}