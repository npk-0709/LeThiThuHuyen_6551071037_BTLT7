using System;
using System.Collections.Generic;

namespace Bai4.Models;

public partial class LoaiPhong
{
    public int MaLoai { get; set; }

    public string TenLoai { get; set; } = null!;

    public decimal? GiaMoiDem { get; set; }

    public string? MoTa { get; set; }

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
