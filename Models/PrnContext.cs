using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PRN_Project.Models;

public partial class PrnContext : DbContext
{
    public static PrnContext INSTANCE = new PrnContext();
    public PrnContext()
    {
    }

    public PrnContext(DbContextOptions<PrnContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Deliver> Delivers { get; set; }

    public virtual DbSet<DeliverInfo> DeliverInfos { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Receive> Receives { get; set; }

    public virtual DbSet<ReceiveInfo> ReceiveInfos { get; set; }

    public virtual DbSet<Suplier> Supliers { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var congfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(congfig.GetConnectionString("DBContext"));
        }

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Deliver>(entity =>
        {
            entity.ToTable("Deliver");

            entity.HasOne(d => d.IdSuplierNavigation).WithMany(p => p.Delivers)
                .HasForeignKey(d => d.IdSuplier)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deliver_Suplier");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Delivers)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deliver_User");
        });

        modelBuilder.Entity<DeliverInfo>(entity =>
        {
            entity.ToTable("DeliverInfo");

            entity.HasOne(d => d.IdDeliverNavigation).WithMany(p => p.DeliverInfos)
                .HasForeignKey(d => d.IdDeliver)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliverInfo_Deliver");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.DeliverInfos)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliverInfo_Product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.DisplayName).HasMaxLength(50);
            entity.Property(e => e.Qrcode).HasColumnName("QRCode");

            entity.HasOne(d => d.IdSuplierNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdSuplier)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Suplier");

            entity.HasOne(d => d.IdUnitNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdUnit)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Unit");
        });

        modelBuilder.Entity<Receive>(entity =>
        {
            entity.ToTable("Receive");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Receives)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Receive_User");
        });

        modelBuilder.Entity<ReceiveInfo>(entity =>
        {
            entity.ToTable("ReceiveInfo");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.ReceiveInfos)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReceiveInfo_Product");

            entity.HasOne(d => d.IdReceiveNavigation).WithMany(p => p.ReceiveInfos)
                .HasForeignKey(d => d.IdReceive)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReceiveInfo_Receive");
        });

        modelBuilder.Entity<Suplier>(entity =>
        {
            entity.ToTable("Suplier");

            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.DisplayName).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.MoreInfo).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.ToTable("Unit");

            entity.Property(e => e.DisplayName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.DisplayName).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserRole");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRole");

            entity.Property(e => e.DisplayName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
