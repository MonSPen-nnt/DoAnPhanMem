﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAPMver1.Data;

public partial class DapmTrangv1Context : DbContext
{
    public DapmTrangv1Context()
    {
    }

    public DapmTrangv1Context(DbContextOptions<DapmTrangv1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<BaoHanh> BaoHanhs { get; set; }

    public virtual DbSet<BinhLuan> BinhLuans { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }

    public virtual DbSet<ChucVu> ChucVus { get; set; }

    public virtual DbSet<DanhGiaSanPham> DanhGiaSanPhams { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KichCo> KichCos { get; set; }

    public virtual DbSet<LichSuGiaSanPham> LichSuGiaSanPhams { get; set; }

    public virtual DbSet<LichSuGiamGiaSanPham> LichSuGiamGiaSanPhams { get; set; }

    public virtual DbSet<MauSac> MauSacs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<SanPhamYeuThich> SanPhamYeuThiches { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<VatLieu> VatLieus { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=laptop-ut0ddbvf;Initial Catalog=DAPM_Trangv1;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaoHanh>(entity =>
        {
            entity.HasKey(e => e.MaBaoHanh).HasName("PK__BaoHanh__A8DF52E5C44F50AF");

            entity.ToTable("BaoHanh");

            entity.Property(e => e.NgayBaoHanh)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.BaoHanhs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BaoHanh__MaNguoi__6B24EA82");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.BaoHanhs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BaoHanh__MaSanPh__6A30C649");
        });

        modelBuilder.Entity<BinhLuan>(entity =>
        {
            entity.HasKey(e => e.MaBinhLuan).HasName("PK__BinhLuan__87CB66A0813B1E7D");

            entity.ToTable("BinhLuan");

            entity.Property(e => e.NgayBinhLuan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BinhLuan__MaNguo__7E37BEF6");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BinhLuan__MaSanP__7D439ABD");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.MaChiTietDonHang).HasName("PK__ChiTietD__4B0B45DD24399AC4");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.MaDonHang)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaDon__787EE5A0");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaSan__797309D9");
        });

        modelBuilder.Entity<ChiTietGioHang>(entity =>
        {
            entity.HasKey(e => new { e.MaGioHang, e.MaSanPham }).HasName("PK__ChiTietG__3AAC69E10208262C");

            entity.ToTable("ChiTietGioHang");

            entity.HasOne(d => d.MaGioHangNavigation).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.MaGioHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietGi__MaGio__06CD04F7");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietGi__MaSan__07C12930");
        });

        modelBuilder.Entity<ChucVu>(entity =>
        {
            entity.HasKey(e => e.MaChucVu).HasName("PK__ChucVu__D463953361A32E6F");

            entity.ToTable("ChucVu");

            entity.Property(e => e.TenChucVu).HasMaxLength(30);
        });

        modelBuilder.Entity<DanhGiaSanPham>(entity =>
        {
            entity.HasKey(e => e.MaDanhGia).HasName("PK__DanhGiaS__AA9515BFB4F4487C");

            entity.ToTable("DanhGiaSanPham");

            entity.Property(e => e.NgayDanhGia)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DanhGiaSanPhams)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGiaSa__MaNgu__208CD6FA");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.DanhGiaSanPhams)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGiaSa__MaSan__1F98B2C1");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc).HasName("PK__DanhMuc__B37508870CF1C467");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.TenDanhMuc).HasMaxLength(50);
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DonHang__129584ADAD9DA7F4");

            entity.ToTable("DonHang");

            entity.Property(e => e.MaDonHang)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DiaChiNguoiNhan).HasMaxLength(100);
            entity.Property(e => e.HinhThucNhanHang)
                .HasMaxLength(50)
                .HasDefaultValue("Giao Hang");
            entity.Property(e => e.SdtnguoiNhan)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SDTNguoiNhan");
            entity.Property(e => e.TenNguoiNhan).HasMaxLength(30);
            entity.Property(e => e.TongSl).HasColumnName("TongSL");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaNguoiGuiNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaNguoiGui)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__MaNguoi__72C60C4A");

            entity.HasOne(d => d.MaVoucherNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaVoucher)
                .HasConstraintName("FK__DonHang__MaVouch__71D1E811");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGioHang).HasName("PK__GioHang__F5001DA3B02C3BE6");

            entity.ToTable("GioHang");

            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasDefaultValue(0.0);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Chua Thanh Toan");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaNguoi__03F0984C");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDon__835ED13B25F77165");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHoaDon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayXuatHoaDon)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaDonHan__18EBB532");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaNguoiD__19DFD96B");

            entity.HasOne(d => d.MaThanhToanNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaThanhToan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaThanhT__1AD3FDA4");
        });

        modelBuilder.Entity<KichCo>(entity =>
        {
            entity.HasKey(e => e.MaKichCo).HasName("PK__KichCo__DE76ED87C485445A");

            entity.ToTable("KichCo");

            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.KichCos)
                .HasForeignKey(d => d.MaSanPham)
                .HasConstraintName("FK__KichCo__MaSanPha__0F624AF8");
        });

        modelBuilder.Entity<LichSuGiaSanPham>(entity =>
        {
            entity.HasKey(e => e.MaLichSu).HasName("PK__LichSuGi__C443222A5E6392B6");

            entity.ToTable("LichSuGiaSanPham");

            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.LichSuGiaSanPhams)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichSuGia__MaSan__29221CFB");
        });

        modelBuilder.Entity<LichSuGiamGiaSanPham>(entity =>
        {
            entity.HasKey(e => e.MaLichSuGiamGia).HasName("PK__LichSuGi__35BD950F25D52449");

            entity.ToTable("LichSuGiamGiaSanPham");

            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.LichSuGiamGiaSanPhams)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichSuGia__MaSan__2CF2ADDF");
        });

        modelBuilder.Entity<MauSac>(entity =>
        {
            entity.HasKey(e => e.MaMauSac).HasName("PK__MauSac__B9A9116268478F7D");

            entity.ToTable("MauSac");

            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.TenMau).HasMaxLength(50);

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.MauSacs)
                .HasForeignKey(d => d.MaSanPham)
                .HasConstraintName("FK__MauSac__MaSanPha__0B91BA14");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D762B8B9461C");

            entity.ToTable("NguoiDung");

            entity.Property(e => e.AnhDaiDien).IsUnicode(false);
            entity.Property(e => e.Sdt)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SDT");
            entity.Property(e => e.TenNguoiDung).HasMaxLength(50);

            entity.HasOne(d => d.MaChucVuNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaChucVu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NguoiDung__MaChu__4F7CD00D");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaTaiKhoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NguoiDung__MaTai__4E88ABD4");
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNhaCungCap).HasName("PK__NhaCungC__53DA92051A6A2A19");

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Sdt)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SDT");
            entity.Property(e => e.TenNhaCungCap).HasMaxLength(100);
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SanPham__FAC7442D54B2BEED");

            entity.ToTable("SanPham");

            entity.Property(e => e.AnhSp)
                .IsUnicode(false)
                .HasColumnName("AnhSP");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TenSanPham).HasMaxLength(50);

            entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaDanhMuc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaDanhM__6477ECF3");

            entity.HasOne(d => d.MaNhaCungCapNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaNhaCungCap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaNhaCu__656C112C");

            entity.HasOne(d => d.MaVatLieuNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaVatLieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaVatLi__6383C8BA");
        });

        modelBuilder.Entity<SanPhamYeuThich>(entity =>
        {
            entity.HasKey(e => e.MaYeuThich).HasName("PK__SanPhamY__B9007E4CC6D8165B");

            entity.ToTable("SanPhamYeuThich");

            entity.Property(e => e.NgayThem)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.SanPhamYeuThiches)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPhamYe__MaNgu__25518C17");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.SanPhamYeuThiches)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPhamYe__MaSan__245D67DE");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TaiKhoan__AD7C652996AB820D");

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaThanhToan).HasName("PK__ThanhToa__D4B258449916AFD5");

            entity.ToTable("ThanhToan");

            entity.Property(e => e.MaDonHang)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayThanhToan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(50);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThanhToan__MaDon__151B244E");
        });

        modelBuilder.Entity<VatLieu>(entity =>
        {
            entity.HasKey(e => e.MaVatLieu).HasName("PK__VatLieu__0A1700AC440A7BE9");

            entity.ToTable("VatLieu");

            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenVatlieu).HasMaxLength(100);
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.MaVoucher).HasName("PK__VOUCHER__0AAC5B1153ACBD08");

            entity.ToTable("VOUCHER");

            entity.Property(e => e.DieuKienApDung).HasDefaultValue(0);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.ThoiGianBatDau)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ThoiGianKetThuc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
