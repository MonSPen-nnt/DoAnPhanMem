﻿using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class KichCo
{
    public int MaKichCo { get; set; }

    public int? MaSanPham { get; set; }

    public int SoKichCo { get; set; }

    public int SoLuong { get; set; }

<<<<<<< HEAD
    public virtual ICollection<BaoHanh> BaoHanhs { get; set; } = new List<BaoHanh>();

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    public virtual SanPham MaSanPhamNavigation { get; set; }
=======
    public virtual SanPham? MaSanPhamNavigation { get; set; }

    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

>>>>>>> bcdbd4465becac8ac1932712dcb28487a1627c7b
}
