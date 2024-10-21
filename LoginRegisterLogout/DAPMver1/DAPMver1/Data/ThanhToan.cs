using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class ThanhToan
{
    public int MaThanhToan { get; set; }

    public string MaDonHang { get; set; } = null!;

    public string PhuongThucThanhToan { get; set; } = null!;

    public DateTime? NgayThanhToan { get; set; }

    public double TongTien { get; set; }

    public bool TrangThaiThanhToan { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;
}
