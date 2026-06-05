using System;
using System.Collections.Generic;

namespace QuanLyNhaHangAPI.Data.Entities;

public partial class NhatKyHeThong
{
    public int MaNhatKy { get; set; }

    public int MaNguoiDung { get; set; }

    public string HanhDong { get; set; } = null!;

    public string ChiTietHanhDong { get; set; } = null!;

    public DateTime? NgayTao { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
