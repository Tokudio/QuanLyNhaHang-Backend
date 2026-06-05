using System;
using System.Collections.Generic;

namespace QuanLyNhaHangAPI.Data.Entities;

public partial class ChiTietDatBan
{
    public int MaChiTietDb { get; set; }

    public int MaDatBan { get; set; }

    public int MaMonAn { get; set; }

    public int SoLuong { get; set; }

    public virtual DatBan MaDatBanNavigation { get; set; } = null!;

    public virtual MonAn MaMonAnNavigation { get; set; } = null!;
}
