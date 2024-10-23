using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class SanPham
{
    public int MaSanPham { get; set; }

    public string TenSanPham { get; set; } = null!;

    public int GiaTienMoi { get; set; }

    public int GiaTienCu { get; set; }

    public string MoTa { get; set; } = null!;

    public string AnhSp { get; set; } = null!;

    public int MaVatLieu { get; set; }

    public int MaDanhMuc { get; set; }

    public DateOnly NgayTao { get; set; }

    public int MaNhaCungCap { get; set; }

    public virtual ICollection<BaoHanh> BaoHanhs { get; set; } = new List<BaoHanh>();

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    public virtual ICollection<DanhGiaSanPham> DanhGiaSanPhams { get; set; } = new List<DanhGiaSanPham>();

    public virtual ICollection<KichCo> KichCos { get; set; } = new List<KichCo>();

    public virtual ICollection<LichSuGiaSanPham> LichSuGiaSanPhams { get; set; } = new List<LichSuGiaSanPham>();

    public virtual ICollection<LichSuGiamGiaSanPham> LichSuGiamGiaSanPhams { get; set; } = new List<LichSuGiamGiaSanPham>();

    public virtual DanhMuc MaDanhMucNavigation { get; set; } = null!;

    public virtual NhaCungCap MaNhaCungCapNavigation { get; set; } = null!;

    public virtual VatLieu MaVatLieuNavigation { get; set; } = null!;

    public virtual ICollection<MauSac> MauSacs { get; set; } = new List<MauSac>();

    public virtual ICollection<SanPhamYeuThich> SanPhamYeuThiches { get; set; } = new List<SanPhamYeuThich>();
}
