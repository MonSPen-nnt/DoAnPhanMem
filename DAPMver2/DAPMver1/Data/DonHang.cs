using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class DonHang
{
    public string MaDonHang { get; set; } = null!;

    public DateOnly NgayDatHang { get; set; }

    public string TrangThai { get; set; } = null!;

    public double PhiVanChuyen { get; set; }

    public double TongTien { get; set; }

    public int MaNguoiGui { get; set; }

    public string SdtnguoiNhan { get; set; } = null!;

    public string DiaChiNguoiNhan { get; set; } = null!;

    public string TenNguoiNhan { get; set; } = null!;

    public int TongSl { get; set; }

    public int TongSoTien { get; set; }

    public int TienPhaiTra { get; set; }

    public int? MaVoucher { get; set; }

    public string HinhThucNhanHang { get; set; } = null!;

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual NguoiDung MaNguoiGuiNavigation { get; set; } = null!;

    public virtual Voucher? MaVoucherNavigation { get; set; }

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
