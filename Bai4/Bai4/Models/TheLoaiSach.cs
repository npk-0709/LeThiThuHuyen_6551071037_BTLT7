using System;
using System.Collections.Generic;

namespace Bai4.Models;

public partial class TheLoaiSach
{
    public int MaTl { get; set; }

    public string TenTheLoai { get; set; } = null!;

    public string? MoTa { get; set; }

    public int? SoLuongSach { get; set; }

    public DateTime? NgayTao { get; set; }
}
