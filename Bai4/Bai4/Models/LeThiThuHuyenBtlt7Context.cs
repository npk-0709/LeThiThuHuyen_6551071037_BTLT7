using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Bai4.Models;

public partial class LeThiThuHuyenBtlt7Context : DbContext
{
    public LeThiThuHuyenBtlt7Context()
    {
    }

    public LeThiThuHuyenBtlt7Context(DbContextOptions<LeThiThuHuyenBtlt7Context> options)
        : base(options)
    {
    }

    public virtual DbSet<BacSi> BacSis { get; set; }

    public virtual DbSet<HoiVien> HoiViens { get; set; }

    public virtual DbSet<LichKham> LichKhams { get; set; }

    public virtual DbSet<LoaiPhong> LoaiPhongs { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<TheLoaiSach> TheLoaiSaches { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=KHUONGSOSAD\\SQLEXPRESS;Database=LeThiThuHuyen_BTLT7;User Id=sa;Password=12345;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BacSi>(entity =>
        {
            entity.HasKey(e => e.MaBs).HasName("PK__BacSi__272475962C3393D0");

            entity.ToTable("BacSi");

            entity.Property(e => e.MaBs).HasColumnName("MaBS");
            entity.Property(e => e.ChuyenKhoa).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<HoiVien>(entity =>
        {
            entity.HasKey(e => e.MaHv).HasName("PK__HoiVien__2725A6D21ED998B2");

            entity.ToTable("HoiVien");

            entity.Property(e => e.MaHv).HasColumnName("MaHV");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HangThanhVien).HasMaxLength(20);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.NgayDangKy)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<LichKham>(entity =>
        {
            entity.HasKey(e => e.MaLich).HasName("PK__LichKham__728A9AE9300424B4");

            entity.ToTable("LichKham");

            entity.Property(e => e.GioKham)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaBs).HasColumnName("MaBS");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.TenBenhNhan).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasMaxLength(20);

            entity.HasOne(d => d.MaBsNavigation).WithMany(p => p.LichKhams)
                .HasForeignKey(d => d.MaBs)
                .HasConstraintName("FK__LichKham__MaBS__31EC6D26");
        });

        modelBuilder.Entity<LoaiPhong>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__LoaiPhon__730A5759239E4DCF");

            entity.ToTable("LoaiPhong");

            entity.Property(e => e.GiaMoiDem).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenLoai).HasMaxLength(100);
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__Phong__20BD5E5B276EDEB3");

            entity.ToTable("Phong");

            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.SoPhong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TinhTrang).HasMaxLength(20);

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.MaLoai)
                .HasConstraintName("FK__Phong__MaLoai__29572725");
        });

        modelBuilder.Entity<TheLoaiSach>(entity =>
        {
            entity.HasKey(e => e.MaTl).HasName("PK__TheLoaiS__27250071145C0A3F");

            entity.ToTable("TheLoaiSach");

            entity.HasIndex(e => e.TenTheLoai, "UQ__TheLoaiS__327F958F173876EA").IsUnique();

            entity.Property(e => e.MaTl).HasColumnName("MaTL");
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoLuongSach).HasDefaultValue(0);
            entity.Property(e => e.TenTheLoai).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
