using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class SanPhamYeuThich
{
    public int MaYeuThich { get; set; }

    public int MaSanPham { get; set; }

    public int MaNguoiDung { get; set; }

    public DateTime? NgayThem { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
