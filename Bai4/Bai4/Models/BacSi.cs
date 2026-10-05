using System;
using System.Collections.Generic;

namespace Bai4.Models;

public partial class BacSi
{
    public int MaBs { get; set; }

    public string HoTen { get; set; } = null!;

    public string? ChuyenKhoa { get; set; }

    public string? Sdt { get; set; }

    public virtual ICollection<LichKham> LichKhams { get; set; } = new List<LichKham>();
}
