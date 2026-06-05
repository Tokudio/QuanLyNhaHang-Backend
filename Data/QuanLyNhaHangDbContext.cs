using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAPI.Data.Entities;

namespace QuanLyNhaHangAPI.Data;

public partial class QuanLyNhaHangDbContext : DbContext
{
    public QuanLyNhaHangDbContext()
    {
    }

    public QuanLyNhaHangDbContext(DbContextOptions<QuanLyNhaHangDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BanAn> BanAn { get; set; }

    public virtual DbSet<ChiTietDatBan> ChiTietDatBan { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHang { get; set; }

    public virtual DbSet<DanhMuc> DanhMuc { get; set; }

    public virtual DbSet<DatBan> DatBan { get; set; }

    public virtual DbSet<DonHang> DonHang { get; set; }

    public virtual DbSet<MonAn> MonAn { get; set; }

    public virtual DbSet<NguoiDung> NguoiDung { get; set; }

    public virtual DbSet<NhatKyHeThong> NhatKyHeThong { get; set; }

    public virtual DbSet<VaiTro> VaiTro { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        //=> optionsBuilder.UseSqlServer("Server=LAPTOP-620PLNS4;Database=QuanLyNhaHangDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BanAn>(entity =>
        {
            entity.HasKey(e => e.MaBan).HasName("PK__BanAn__3520ED6C66A02E56");

            entity.HasIndex(e => e.SoBan, "UQ__BanAn__21B4EECB14171628").IsUnique();

            entity.Property(e => e.SoBan).HasMaxLength(20);
            entity.Property(e => e.TrangThaiBan)
                .HasMaxLength(50)
                .HasDefaultValue("Trống");
        });

        modelBuilder.Entity<ChiTietDatBan>(entity =>
        {
            entity.HasKey(e => e.MaChiTietDb).HasName("PK__ChiTietD__651E6E6C26C0E398");

            entity.Property(e => e.MaChiTietDb).HasColumnName("MaChiTietDB");

            entity.HasOne(d => d.MaDatBanNavigation).WithMany(p => p.ChiTietDatBan)
                .HasForeignKey(d => d.MaDatBan)
                .HasConstraintName("FK__ChiTietDa__MaDat__571DF1D5");

            entity.HasOne(d => d.MaMonAnNavigation).WithMany(p => p.ChiTietDatBan)
                .HasForeignKey(d => d.MaMonAn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDa__MaMon__5812160E");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.MaChiTiet).HasName("PK__ChiTietD__CDF0A114A851F33A");

            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.GiaLucDat).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThaiBep)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ chế biến");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHang)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__ChiTietDo__MaDon__6D0D32F4");

            entity.HasOne(d => d.MaMonAnNavigation).WithMany(p => p.ChiTietDonHang)
                .HasForeignKey(d => d.MaMonAn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaMon__6E01572D");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc).HasName("PK__DanhMuc__B37508872B5A7360");

            entity.HasIndex(e => e.TenDanhMuc, "UQ__DanhMuc__650CAE4EFFE4978C").IsUnique();

            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
        });

        modelBuilder.Entity<DatBan>(entity =>
        {
            entity.HasKey(e => e.MaDatBan).HasName("PK__DatBan__703DFB75603E0AD1");

            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThaiDat)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ xác nhận");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.DatBan)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatBan__MaKhachH__534D60F1");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DonHang__129584ADAE017976");

            entity.Property(e => e.LoaiDonHang).HasMaxLength(50);
            entity.Property(e => e.MaGiaoDichVnpay)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MaGiaoDichVNPay");
            entity.Property(e => e.MaPhanHoiNganHang)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(50);
            entity.Property(e => e.ThoiGianHenLay).HasColumnType("datetime");
            entity.Property(e => e.TongTien)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThaiDon)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ xác nhận");
            entity.Property(e => e.TrangThaiThanhToan)
                .HasMaxLength(50)
                .HasDefaultValue("Chưa thanh toán");

            entity.HasOne(d => d.MaBanNavigation).WithMany(p => p.DonHang)
                .HasForeignKey(d => d.MaBan)
                .HasConstraintName("FK__DonHang__MaBan__6477ECF3");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.DonHangMaKhachHangNavigation)
                .HasForeignKey(d => d.MaKhachHang)
                .HasConstraintName("FK__DonHang__MaKhach__6383C8BA");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.DonHangMaNhanVienNavigation)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__DonHang__MaNhanV__656C112C");
        });

        modelBuilder.Entity<MonAn>(entity =>
        {
            entity.HasKey(e => e.MaMonAn).HasName("PK__MonAn__B1171625DD660392");

            entity.Property(e => e.ConBan).HasDefaultValue(true);
            entity.Property(e => e.Gia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HinhAnh)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TenMonAn).HasMaxLength(150);

            entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.MonAn)
                .HasForeignKey(d => d.MaDanhMuc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MonAn__MaDanhMuc__46E78A0C");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D7626CE77E9A");

            entity.HasIndex(e => e.DienThoai, "UQ__NguoiDun__1F031876DECFFF69").IsUnique();

            entity.HasIndex(e => e.TenDangNhap, "UQ__NguoiDun__55F68FC05CD528C9").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__NguoiDun__A9D1053480920FFD").IsUnique();

            entity.Property(e => e.DienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TrangThaiHoatDong).HasDefaultValue(false);

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.NguoiDung)
                .HasForeignKey(d => d.MaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NguoiDung__MaVai__3F466844");
        });

        modelBuilder.Entity<NhatKyHeThong>(entity =>
        {
            entity.HasKey(e => e.MaNhatKy).HasName("PK__NhatKyHe__E42EF42E709CAD10");

            entity.Property(e => e.HanhDong).HasMaxLength(100);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.NhatKyHeThong)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NhatKyHeT__MaNgu__71D1E811");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VaiTro__C24C41CFDDB39929");

            entity.HasIndex(e => e.TenVaiTro, "UQ__VaiTro__1DA5581462E0C436").IsUnique();

            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
