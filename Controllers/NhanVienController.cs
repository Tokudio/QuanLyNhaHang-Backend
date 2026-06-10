using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHangAPI.Models;
using QuanLyNhaHangAPI.Services;

namespace QuanLyNhaHangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Nhân viên,Admin")]
    public class NhanVienController : ControllerBase
    {
        private readonly INhanVienService _nhanVienService;

        public NhanVienController(INhanVienService nhanVienService)
        {
            _nhanVienService = nhanVienService;
        }

        // [GET] /api/nhanvien/ban-an
        [HttpGet("ban-an")]
        public async Task<IActionResult> GetDanhSachBanAn()
        {
            var result = await _nhanVienService.LayDanhSachBanAnAsync();
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // [GET] /api/nhanvien/don-hang-moi
        [HttpGet("don-hang-moi")]
        public async Task<IActionResult> GetDonHangMoi()
        {
            var result = await _nhanVienService.LayDonHangMoiAsync();
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // [GET] /api/nhanvien/ban/{maBan}/don-hang
        [HttpGet("ban/{maBan}/don-hang")]
        public async Task<IActionResult> GetDonHangTheoBan(int maBan)
        {
            var result = await _nhanVienService.LayDonHangHienTaiTheoBanAsync(maBan);
            if (!result.IsSuccess) return NotFound(result); // Trả về 404 nếu bàn trống
            return Ok(result.Data); // Trả thẳng Data để Model Angular nhận (DonHangHienTaiDto)
        }

        // [PUT] /api/nhanvien/chi-tiet/{maChiTiet}/so-luong
        [HttpPut("chi-tiet/{maChiTiet}/so-luong")]
        public async Task<IActionResult> UpdateSoLuongMon(int maChiTiet, [FromBody] CapNhatSoLuongRequest request)
        {
            var result = await _nhanVienService.CapNhatSoLuongMonAsync(maChiTiet, request.SoLuongMoi);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // [POST] /api/nhanvien/don-hang/them-mon
        [HttpPost("don-hang/them-mon")]
        public async Task<IActionResult> AddMonVaoDon([FromBody] ThemMonVaoDonRequest request)
        {
            var result = await _nhanVienService.ThemMonVaoDonHienTaiAsync(request);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // [PUT] /api/nhanvien/don-hang/{maDonHang}/thanh-toan
        [HttpPut("don-hang/{maDonHang}/thanh-toan")]
        public async Task<IActionResult> ThanhToanDonHang(int maDonHang)
        {
            var result = await _nhanVienService.XacNhanThanhToanAsync(maDonHang);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}