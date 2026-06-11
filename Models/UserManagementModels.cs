namespace QuanLyNhaHangAPI.Models
{
    public class CreateEmployeeRequest
    {
        public string HoTen { get; set; } = "";
        public string Email { get; set; } = "";
        public string DienThoai { get; set; } = "";
        public string MatKhau { get; set; } = "";
        public int MaVaiTro { get; set; }
    }

    public class UpdateRoleRequest
    {
        public int MaVaiTro { get; set; }
    }

    public class UpdateStatusRequest
    {
        public bool TrangThaiHoatDong { get; set; }
    }
}