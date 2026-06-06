using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHangAPI.Data;
using QuanLyNhaHangAPI.Data.Entities;
using QuanLyNhaHangAPI.Models;

namespace QuanLyNhaHangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonHangController : ControllerBase
    {
        private readonly QuanLyNhaHangDbContext _context;

        public DonHangController(QuanLyNhaHangDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDTO request)
        {
            try
            {
                if (request == null || !request.ChiTietDonHang.Any())
                {
                    return BadRequest(new { isSuccess = false, message = "Giỏ hàng trống!" });
                }

                // 1. Tạo đơn hàng mới để lưu vào bảng DonHang
                var donHangMoi = new DonHang
                {
                    NgayTao = DateTime.Now,
                    TrangThaiDon = "Chờ xác nhận",       // Đã sửa lại đúng tên cột trong Entity
                    TrangThaiThanhToan = "Chưa thanh toán", // Thêm trạng thái thanh toán mặc định
                    LoaiDonHang = request.LoaiDonHang,
                    PhuongThucThanhToan = request.PhuongThucThanhToan,
                    TongTien = request.ChiTietDonHang.Sum(x => x.SoLuong * x.GiaLucDat)
                };

                // Lưu ý: Nếu gạch đỏ, hãy bỏ chữ 's' đi (thành _context.DonHang) tùy cấu hình DbContext của cậu
                _context.DonHang.Add(donHangMoi);
                await _context.SaveChangesAsync(); // Lưu để EF Core tự sinh ra ID (MaDonHang)

                // 2. Tạo chi tiết đơn hàng lưu vào bảng ChiTietDonHang
                foreach (var item in request.ChiTietDonHang)
                {
                    var chiTiet = new ChiTietDonHang
                    {
                        MaDonHang = donHangMoi.MaDonHang, // Lấy ID vừa được tạo ở trên
                        MaMonAn = item.MaMonAn,
                        SoLuong = item.SoLuong,
                        GiaLucDat = item.GiaLucDat,       // Đã sửa lại đúng tên cột (GiaLucDat thay vì DonGia)
                        TrangThaiBep = "Chờ chế biến",    // Thêm trạng thái bếp để Tiến dễ quản lý
                        NgayTao = DateTime.Now
                    };
                    // Lưu ý: Nếu gạch đỏ, hãy bỏ chữ 's' đi (thành _context.ChiTietDonHang)
                    _context.ChiTietDonHang.Add(chiTiet);
                }

                await _context.SaveChangesAsync();

                // 3. Trả kết quả về cho Angular (Frontend)
                return Ok(new { isSuccess = true, message = "Đặt món thành công! Đơn hàng đã chuyển xuống bếp." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}