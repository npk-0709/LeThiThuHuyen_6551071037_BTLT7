using System;
using System.Collections.Generic;

namespace Bai3.Models;

public partial class HoiVien
{
    public int MaHv { get; set; }

    public string HoTen { get; set; } = null!;

    public bool? GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public string? HangThanhVien { get; set; }

    public DateTime? NgayDangKy { get; set; }

    public bool? TrangThai { get; set; }
}
