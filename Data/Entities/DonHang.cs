using System;
using System.Collections.Generic;

namespace QuanLyNhaHangAPI.Data.Entities;

public partial class DonHang
{
    public int MaDonHang { get; set; }

    public int? MaKhachHang { get; set; }

    public int? MaBan { get; set; }

    public int? MaNhanVien { get; set; }

    public string LoaiDonHang { get; set; } = null!;

    public DateTime? ThoiGianHenLay { get; set; }

    public decimal? TongTien { get; set; }

    public string? TrangThaiDon { get; set; }

    public string PhuongThucThanhToan { get; set; } = null!;

    public string? TrangThaiThanhToan { get; set; }

    public string? MaGiaoDichVnpay { get; set; }

    public string? MaPhanHoiNganHang { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHang { get; set; } = new List<ChiTietDonHang>();

    public virtual BanAn? MaBanNavigation { get; set; }

    public virtual NguoiDung? MaKhachHangNavigation { get; set; }

    public virtual NguoiDung? MaNhanVienNavigation { get; set; }
}
