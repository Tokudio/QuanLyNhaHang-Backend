using System;
using System.Collections.Generic;

namespace QuanLyNhaHangAPI.Data.Entities;

public partial class MonAn
{
    public int MaMonAn { get; set; }

    public int MaDanhMuc { get; set; }

    public string TenMonAn { get; set; } = null!;

    public string? HinhAnh { get; set; }

    public decimal Gia { get; set; }

    public string? MoTa { get; set; }

    public bool? ConBan { get; set; }

    public virtual ICollection<ChiTietDatBan> ChiTietDatBan { get; set; } = new List<ChiTietDatBan>();

    public virtual ICollection<ChiTietDonHang> ChiTietDonHang { get; set; } = new List<ChiTietDonHang>();

    public virtual DanhMuc MaDanhMucNavigation { get; set; } = null!;
}
