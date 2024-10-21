using System;
using System.Collections.Generic;

namespace BanTrangSuc.Data;

public partial class HoaDon
{
    public string MaHoaDon { get; set; } = null!;

    public string MaDonHang { get; set; } = null!;

    public int MaNguoiDung { get; set; }

    public DateTime? NgayXuatHoaDon { get; set; }

    public double TongTien { get; set; }

    public int MaThanhToan { get; set; }

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    public virtual ThanhToan MaThanhToanNavigation { get; set; } = null!;
}
