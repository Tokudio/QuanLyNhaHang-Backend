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

                string loaiDH = request.LoaiDonHang;
                string ptThanhToan = request.PhuongThucThanhToan;

                // 1. Tạo đơn hàng mới dựa trên dữ liệu người dùng chọn
                var donHangMoi = new DonHang
                {
                    NgayTao = DateTime.Now,
                    TrangThaiDon = "Chờ xác nhận",
                    TrangThaiThanhToan = "Chưa thanh toán",
                    LoaiDonHang = loaiDH,
                    PhuongThucThanhToan = ptThanhToan,
                    TongTien = request.ChiTietDonHang.Sum(x => x.SoLuong * x.GiaLucDat),

                    // NẾU TẠI QUÁN THÌ LƯU MÃ BÀN, NẾU MANG VỀ THÌ LƯU HẸN LẤY
                    MaBan = loaiDH == "Tại quán" ? request.MaBan : null,
                    ThoiGianHenLay = loaiDH == "Mang về" ? DateTime.Now.AddMinutes(15) : null
                };

                _context.DonHang.Add(donHangMoi);
                await _context.SaveChangesAsync();

                // 2. Tạo chi tiết đơn hàng
                foreach (var item in request.ChiTietDonHang)
                {
                    var chiTiet = new ChiTietDonHang
                    {
                        MaDonHang = donHangMoi.MaDonHang,
                        MaMonAn = item.MaMonAn,
                        SoLuong = item.SoLuong,
                        GiaLucDat = item.GiaLucDat,
                        TrangThaiBep = "Chờ chế biến",
                        NgayTao = DateTime.Now,
                        GhiChu = string.IsNullOrEmpty(request.GhiChu) ? "" : request.GhiChu
                    };
                    _context.ChiTietDonHang.Add(chiTiet);
                }

                await _context.SaveChangesAsync();

                return Ok(new { isSuccess = true, message = "Đặt món thành công! Đơn hàng đã chuyển xuống bếp." });
            }
            catch (Exception ex)
            {
                var loiChiTiet = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { isSuccess = false, message = "Lỗi hệ thống: " + loiChiTiet });
            }
        }
    }
}