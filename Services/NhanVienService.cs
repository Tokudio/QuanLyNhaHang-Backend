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

        // 1. Lấy danh sách bàn ăn
        public async Task<NhanVienApiResponse<List<BanAnDto>>> LayDanhSachBanAnAsync()
        {
            try
            {
                var banAns = await _context.BanAn
                    .Select(b => new BanAnDto
                    {
                        MaBan = b.MaBan,
                        SoBan = b.SoBan,
                        TrangThaiBan = b.TrangThaiBan
                    })
                    .ToListAsync();

                return new NhanVienApiResponse<List<BanAnDto>> { IsSuccess = true, Message = "Thành công", Data = banAns };
            }
            catch (Exception ex)
            {
                return new NhanVienApiResponse<List<BanAnDto>> { IsSuccess = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        // 2. Lấy đơn hàng mới
        public async Task<NhanVienApiResponse<List<DonHangNhanVienDto>>> LayDonHangMoiAsync()
        {
            try
            {
                var donHangs = await _context.DonHang
                    .Include(d => d.MaBanNavigation)
                    .Where(d => d.TrangThaiDon == "Chờ xác nhận" || d.TrangThaiDon == "Đang chuẩn bị")
                    .OrderByDescending(d => d.NgayTao)
                    .Select(d => new DonHangNhanVienDto
                    {
                        MaDonHang = d.MaDonHang,
                        MaBan = d.MaBan,
                        TenBan = d.MaBanNavigation != null ? d.MaBanNavigation.SoBan : "Mang về",
                        LoaiDonHang = d.LoaiDonHang,
                        NgayTao = d.NgayTao,
                        TongTien = d.TongTien,
                        TrangThaiDon = d.TrangThaiDon
                    })
                    .ToListAsync();

                return new NhanVienApiResponse<List<DonHangNhanVienDto>> { IsSuccess = true, Message = "Thành công", Data = donHangs };
            }
            catch (Exception ex)
            {
                return new NhanVienApiResponse<List<DonHangNhanVienDto>> { IsSuccess = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        // 3. Lấy đơn hàng hiện tại theo bàn (Xử lý bàn tại quán hoặc Mang về)
        public async Task<NhanVienApiResponse<DonHangHienTaiDto>> LayDonHangHienTaiTheoBanAsync(int maBan)
        {
            try
            {
                DonHang? donHang = null;

                if (maBan <= 0)
                {
                    donHang = await _context.DonHang
                        .Include(d => d.ChiTietDonHang)
                        .ThenInclude(c => c.MaMonAnNavigation)
                        .Where(d => d.MaBan == null && d.LoaiDonHang == "Mang về" && (d.TrangThaiThanhToan == "Chưa thanh toán" || d.TrangThaiThanhToan == null))
                        .OrderByDescending(d => d.NgayTao)
                        .FirstOrDefaultAsync();
                }
                else
                {
                    donHang = await _context.DonHang
                        .Include(d => d.MaBanNavigation)
                        .Include(d => d.ChiTietDonHang)
                        .ThenInclude(c => c.MaMonAnNavigation)
                        .Where(d => d.MaBan == maBan && (d.TrangThaiThanhToan == "Chưa thanh toán" || d.TrangThaiThanhToan == null))
                        .OrderByDescending(d => d.NgayTao)
                        .FirstOrDefaultAsync();
                }

                if (donHang == null)
                {
                    return new NhanVienApiResponse<DonHangHienTaiDto> { IsSuccess = false, Message = "Bàn này hiện chưa có hóa đơn đang xử lý." };
                }

                var donHangDto = new DonHangHienTaiDto
                {
                    MaDonHang = donHang.MaDonHang,
                    MaBan = donHang.MaBan,
                    SoBan = donHang.MaBan.HasValue ? donHang.MaBanNavigation?.SoBan : "Mang về",
                    TongTien = donHang.TongTien ?? 0,
                    TrangThaiDon = donHang.TrangThaiDon,
                    TrangThaiThanhToan = donHang.TrangThaiThanhToan,
                    ChiTietDonHangs = donHang.ChiTietDonHang.Select(c => new ChiTietDonHangDto
                    {
                        MaChiTiet = c.MaChiTiet,
                        MaMonAn = c.MaMonAn,
                        TenMonAn = c.MaMonAnNavigation != null ? c.MaMonAnNavigation.TenMonAn : "Món đã xóa",
                        SoLuong = c.SoLuong,       // ĐÃ SỬA: Bỏ ?? 0 vì thuộc tính thực thể là kiểu 'int'
                        GiaLucDat = c.GiaLucDat,   // ĐÃ SỬA: Bỏ ?? 0 vì thuộc tính thực thể là kiểu 'decimal'
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

        // 4. Cập nhật số lượng món
        public async Task<NhanVienApiResponse<bool>> CapNhatSoLuongMonAsync(int maChiTiet, int soLuongMoi)
        {
            try
            {
                var chiTiet = await _context.ChiTietDonHang.FindAsync(maChiTiet);
                if (chiTiet == null) return new NhanVienApiResponse<bool> { IsSuccess = false, Message = "Chi tiết đơn hàng không tồn tại." };

                int maDonHang = chiTiet.MaDonHang; // ĐÃ SỬA: Bỏ ?? 0 vì thuộc tính thực thể là kiểu 'int'

                if (soLuongMoi <= 0)
                {
                    _context.ChiTietDonHang.Remove(chiTiet);
                }
                else
                {
                    chiTiet.SoLuong = soLuongMoi;
                    _context.ChiTietDonHang.Update(chiTiet);
                }

                await _context.SaveChangesAsync();
                await TinhLaiTongTienDonHangAsync(maDonHang);

                return new NhanVienApiResponse<bool> { IsSuccess = true, Message = "Cập nhật thành công.", Data = true };
            }
            catch (Exception ex)
            {
                return new NhanVienApiResponse<bool> { IsSuccess = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        // 5. Thêm món vào đơn hiện tại (Tự tạo đơn nếu bàn trống)
        public async Task<NhanVienApiResponse<int>> ThemMonVaoDonHienTaiAsync(ThemMonVaoDonRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var monAn = await _context.MonAn.FindAsync(request.MaMonAn);
                if (monAn == null) return new NhanVienApiResponse<int> { IsSuccess = false, Message = "Món ăn không tồn tại." };

                int maDonHang = request.MaDonHang;

                if (maDonHang <= 0)
                {
                    var donHangMoi = new DonHang
                    {
                        MaBan = (request.MaBan.HasValue && request.MaBan > 0) ? request.MaBan : null,
                        LoaiDonHang = (request.MaBan.HasValue && request.MaBan > 0) ? "Tại quán" : "Mang về",
                        TrangThaiDon = "Đang chuẩn bị",
                        TrangThaiThanhToan = "Chưa thanh toán",
                        PhuongThucThanhToan = "Tiền mặt",
                        NgayTao = DateTime.Now,
                        TongTien = 0
                    };

                    await _context.DonHang.AddAsync(donHangMoi);
                    await _context.SaveChangesAsync();
                    maDonHang = donHangMoi.MaDonHang;

                    if (request.MaBan.HasValue && request.MaBan > 0)
                    {
                        var banAn = await _context.BanAn.FindAsync(request.MaBan);
                        if (banAn != null)
                        {
                            banAn.TrangThaiBan = "Đang phục vụ";
                            _context.BanAn.Update(banAn);
                        }
                    }
                }

                var chiTietTonTai = await _context.ChiTietDonHang
                    .FirstOrDefaultAsync(c => c.MaDonHang == maDonHang && c.MaMonAn == request.MaMonAn);

                if (chiTietTonTai != null)
                {
                    chiTietTonTai.SoLuong = chiTietTonTai.SoLuong + request.SoLuong; // ĐÃ SỬA: Bỏ ?? 0 vì thuộc tính thực thể là kiểu 'int'
                    _context.ChiTietDonHang.Update(chiTietTonTai);
                }
                else
                {
                    var chiTietMoi = new ChiTietDonHang
                    {
                        MaDonHang = maDonHang,
                        MaMonAn = request.MaMonAn,
                        SoLuong = request.SoLuong,
                        GiaLucDat = monAn.Gia,
                        TrangThaiBep = "Chờ chế biến"
                    };
                    await _context.ChiTietDonHang.AddAsync(chiTietMoi);
                }

                await _context.SaveChangesAsync();
                await TinhLaiTongTienDonHangAsync(maDonHang);

                await transaction.CommitAsync();

                return new NhanVienApiResponse<int> { IsSuccess = true, Message = "Thêm món thành công.", Data = maDonHang };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new NhanVienApiResponse<int> { IsSuccess = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        // 6. Xác nhận thanh toán và giải phóng bàn
        public async Task<NhanVienApiResponse<bool>> XacNhanThanhToanAsync(int maDonHang)
        {
            try
            {
                var donHang = await _context.DonHang.FindAsync(maDonHang);
                if (donHang == null) return new NhanVienApiResponse<bool> { IsSuccess = false, Message = "Không tìm thấy đơn hàng." };

                donHang.TrangThaiThanhToan = "Đã thanh toán online";
                donHang.TrangThaiDon = "Hoàn thành";
                _context.DonHang.Update(donHang);

                if (donHang.MaBan != null)
                {
                    var banAn = await _context.BanAn.FindAsync(donHang.MaBan);
                    if (banAn != null)
                    {
                        banAn.TrangThaiBan = "Trống";
                        _context.BanAn.Update(banAn);
                    }
                }

                await _context.SaveChangesAsync();
                return new NhanVienApiResponse<bool> { IsSuccess = true, Message = "Thanh toán thành công.", Data = true };
            }
            catch (Exception ex)
            {
                return new NhanVienApiResponse<bool> { IsSuccess = false, Message = $"Lỗi: {ex.InnerException?.Message ?? ex.Message}" };
            }
        }

        // --- Hàm helper tính tổng tiền ---
        private async Task TinhLaiTongTienDonHangAsync(int maDonHang)
        {
            var donHang = await _context.DonHang
                .Include(d => d.ChiTietDonHang)
                .FirstOrDefaultAsync(d => d.MaDonHang == maDonHang);

            if (donHang != null)
            {
                // ĐÃ SỬA: Loại bỏ toàn bộ toán tử ?? thừa kế từ kiểu dữ liệu gốc nguyên bản
                donHang.TongTien = donHang.ChiTietDonHang.Sum(c => c.SoLuong * c.GiaLucDat);
                _context.DonHang.Update(donHang);
                await _context.SaveChangesAsync();
            }
        }
    }
}