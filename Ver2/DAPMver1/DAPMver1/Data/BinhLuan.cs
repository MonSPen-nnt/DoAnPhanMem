using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class BinhLuan
{
    public int MaBinhLuan { get; set; }

    public int MaSanPham { get; set; }

    public int MaNguoiDung { get; set; }

    public string NoiDung { get; set; } = null!;

    public DateTime NgayBinhLuan { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
