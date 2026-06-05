using System;
using System.Collections.Generic;

namespace QuanLyNhaHangAPI.Data.Entities;

public partial class BanAn
{
    public int MaBan { get; set; }

    public string SoBan { get; set; } = null!;

    public int SucChua { get; set; }

    public string? TrangThaiBan { get; set; }

    public virtual ICollection<DonHang> DonHang { get; set; } = new List<DonHang>();
}
