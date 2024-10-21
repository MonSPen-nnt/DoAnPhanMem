using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class ChiTietDonHang
{
    public int MaChiTietDonHang { get; set; }

    public string MaDonHang { get; set; } = null!;

    public int MaSanPham { get; set; }

    public int Soluong { get; set; }

    public int DonGia { get; set; }

    public int TongTien { get; set; }

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
