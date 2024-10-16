using System;
using System.Collections.Generic;

namespace BanTrangSuc.Data;

public partial class DanhGiaSanPham
{
    public int MaDanhGia { get; set; }

    public int MaSanPham { get; set; }

    public int MaNguoiDung { get; set; }

    public int DiemDanhGia { get; set; }

    public DateTime? NgayDanhGia { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
