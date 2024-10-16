using System;
using System.Collections.Generic;

namespace BanTrangSuc.Data;

public partial class ChucVu
{
    public int MaChucVu { get; set; }

    public string? TenChucVu { get; set; }

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
}
