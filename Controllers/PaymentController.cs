using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAPI.Data;
using QuanLyNhaHangAPI.Data.Entities;
using QuanLyNhaHangAPI.Models;
using QuanLyNhaHangAPI.Services;

namespace QuanLyNhaHangAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly VNPayService _vnPayService;
    private readonly QuanLyNhaHangDbContext _context;
    public PaymentController(VNPayService vnPayService, QuanLyNhaHangDbContext context)
    {
        _vnPayService = vnPayService;
        _context = context;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] OrderRequestDTO request)
    {
        try
        {
            // === DEBUG PAYLOAD ===
            var requestJson = System.Text.Json.JsonSerializer.Serialize(request);
            Console.WriteLine("=== REQUEST PAYLOAD ===");
            Console.WriteLine(requestJson);

            if (request == null || !request.ChiTietDonHang.Any())
                return BadRequest(new { isSuccess = false, message = "Giỏ hàng trống!" });

            string paymentMethod = (request.PhuongThucThanhToan ?? "").Trim();

            Console.WriteLine($"[DEBUG] PhuongThucThanhToan nhận được = '{paymentMethod}'");

            if (!paymentMethod.Equals("VNPay", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { isSuccess = false, message = $"Phương thức không hợp lệ: {paymentMethod}" });

            // ... (phần code tạo orderId, paymentUrl, donHang giữ nguyên như cũ)

            string orderId = "DH" + DateTime.Now.Ticks.ToString();
            string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            // THÊM ĐOẠN NÀY: Nếu là IPv6 localhost thì chuyển về IPv4 chuẩn
            if (clientIp == "::1")
            {
                clientIp = "127.0.0.1";
            }

            string paymentUrl = _vnPayService.CreatePaymentUrl(request, orderId, clientIp);

            var donHang = new DonHang
            {
                NgayTao = DateTime.Now,
                TrangThaiDon = "Chờ xác nhận",
                TrangThaiThanhToan = "Chưa thanh toán",
                LoaiDonHang = request.LoaiDonHang ?? "Mang về",
                PhuongThucThanhToan = "VNPay",
                TongTien = request.ChiTietDonHang.Sum(x => x.SoLuong * x.GiaLucDat),
                MaBan = request.LoaiDonHang == "Tại quán" ? request.MaBan : null,
                MaGiaoDichVnpay = orderId,
                MaKhachHang = null,
                MaNhanVien = null,
                ThoiGianHenLay = request.LoaiDonHang == "Mang về" ? DateTime.Now.AddMinutes(30) : null
            };

            _context.DonHang.Add(donHang);
            await _context.SaveChangesAsync();

            foreach (var item in request.ChiTietDonHang)
            {
                _context.ChiTietDonHang.Add(new ChiTietDonHang
                {
                    MaDonHang = donHang.MaDonHang,
                    MaMonAn = item.MaMonAn,
                    SoLuong = item.SoLuong,
                    GiaLucDat = item.GiaLucDat,
                    TrangThaiBep = "Chờ chế biến",
                    NgayTao = DateTime.Now,
                    GhiChu = request.GhiChu ?? ""
                });
            }
            await _context.SaveChangesAsync();

            return Ok(new { isSuccess = true, paymentUrl, orderId, message = "Tạo link thanh toán thành công" });
        }
        catch (Exception ex)
        {
            Console.WriteLine("=== VNPAY ERROR ===\n" + ex.ToString());
            return StatusCode(500, new { isSuccess = false, message = ex.Message });
        }
    }

    [HttpGet("vnpay-return")]
    public async Task<IActionResult> VnpayReturn()
    {
        if (!_vnPayService.ValidateSignature(Request.Query))
        {
            return Redirect("/payment-failed?message=Invalid signature");
        }

        string vnp_TxnRef = Request.Query["vnp_TxnRef"].ToString();
        string vnp_ResponseCode = Request.Query["vnp_ResponseCode"].ToString();

        var donHang = await _context.DonHang.FirstOrDefaultAsync(x => x.MaGiaoDichVnpay == vnp_TxnRef);

        if (donHang != null)
        {
            if (vnp_ResponseCode == "00")
            {
                donHang.TrangThaiThanhToan = "Đã thanh toán";
                donHang.TrangThaiDon = "Chờ xác nhận";
            }
            else
            {
                donHang.TrangThaiDon = "Thanh toán thất bại";
            }
            await _context.SaveChangesAsync();
        }

        return Redirect(vnp_ResponseCode == "00"
            ? $"/payment-success?orderId={vnp_TxnRef}"
            : "/payment-failed");
    }
}