using System;
using System.Collections.Generic;

namespace QuanLyNhaHangAPI.Data.Entities;

public partial class NguoiDung
{
    public int MaNguoiDung { get; set; }

    public int MaVaiTro { get; set; }

    public string HoTen { get; set; } = null!;

    public string? Email { get; set; }

    public string? DienThoai { get; set; }

    public string? TenDangNhap { get; set; }

    public string MatKhau { get; set; } = null!;

    public bool? TrangThaiHoatDong { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<DatBan> DatBan { get; set; } = new List<DatBan>();

    public virtual ICollection<DonHang> DonHangMaKhachHangNavigation { get; set; } = new List<DonHang>();

    public virtual ICollection<DonHang> DonHangMaNhanVienNavigation { get; set; } = new List<DonHang>();

    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;

    public virtual ICollection<NhatKyHeThong> NhatKyHeThong { get; set; } = new List<NhatKyHeThong>();
}
