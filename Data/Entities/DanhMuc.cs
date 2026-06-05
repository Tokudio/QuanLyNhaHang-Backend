using System;
using System.Collections.Generic;

namespace QuanLyNhaHangAPI.Data.Entities;

public partial class DanhMuc
{
    public int MaDanhMuc { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public virtual ICollection<MonAn> MonAn { get; set; } = new List<MonAn>();
}
