using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAPI.Data; // Chứa QuanLyNhaHangDbContext
using QuanLyNhaHangAPI.Data.Entities; // Chứa bảng MonAn

namespace QuanLyNhaHangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonAnController : ControllerBase
    {
        private readonly QuanLyNhaHangDbContext _context;

        public MonAnController(QuanLyNhaHangDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenu()
        {
            // Lấy danh sách món ăn từ Database
            // (Lưu ý: Nếu _context.MonAn báo lỗi đỏ, cậu thử đổi thành _context.MonAns nhé, tùy thuộc vào cách cậu cấu hình trong DbContext)
            var danhSachMonAn = await _context.MonAn.ToListAsync();

            return Ok(danhSachMonAn);
        }
    }
}