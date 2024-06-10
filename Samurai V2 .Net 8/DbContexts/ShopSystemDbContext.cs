using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Samurai_V2_.Net_8.DbContexts.Models;

namespace Samurai_V2_.Net_8.DbContexts;

public partial class ShopSystemDbContext : DbContext
{
    public ShopSystemDbContext(DbContextOptions<ShopSystemDbContext> options): base(options)
    {

    }

    public virtual DbSet<TblItem> TblItems { get; set; }

    public virtual DbSet<TblLogin> TblLogins { get; set; }

    public virtual DbSet<TblPartner> TblPartners { get; set; }

    public virtual DbSet<TblPartnerType> TblPartnerTypes { get; set; }

    public virtual DbSet<TblPurchase> TblPurchases { get; set; }

    public virtual DbSet<TblPurchaseDetail> TblPurchaseDetails { get; set; }

    public virtual DbSet<TblSale> TblSales { get; set; }

    public virtual DbSet<TblSalesDetail> TblSalesDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=DESKTOP-4KCTB07;Initial Catalog=ShopSystem;Integrated Security=True;Trust Server Certificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblPartner>(entity =>
        {
            entity.HasKey(e => e.PartnerId).HasName("PK_Partner");

            entity.HasOne(d => d.PartnerType).WithMany(p => p.TblPartners)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPartner_tblPartnerType");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
