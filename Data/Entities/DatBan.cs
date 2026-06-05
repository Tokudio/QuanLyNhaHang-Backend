using System;
using System.Collections.Generic;

namespace QuanLyNhaHangAPI.Data.Entities;

public partial class DatBan
{
    public int MaDatBan { get; set; }

    public int MaKhachHang { get; set; }

    public DateOnly NgayDen { get; set; }

    public TimeOnly GioDen { get; set; }

    public int SoLuongKhach { get; set; }

    public string? GhiChu { get; set; }

    public string? TrangThaiDat { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<ChiTietDatBan> ChiTietDatBan { get; set; } = new List<ChiTietDatBan>();

    public virtual NguoiDung MaKhachHangNavigation { get; set; } = null!;
}
