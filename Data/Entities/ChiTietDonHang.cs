using System;
using System.Collections.Generic;

namespace QuanLyNhaHangAPI.Data.Entities;

public partial class ChiTietDonHang
{
    public int MaChiTiet { get; set; }

    public int MaDonHang { get; set; }

    public int MaMonAn { get; set; }

    public int SoLuong { get; set; }

    public decimal GiaLucDat { get; set; }

    public string? GhiChu { get; set; }

    public string? TrangThaiBep { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;

    public virtual MonAn MaMonAnNavigation { get; set; } = null!;
}
