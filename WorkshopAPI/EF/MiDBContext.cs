using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WorkshopAPI.EF;

public partial class MiDBContext : DbContext
{
    public MiDBContext()
    {
    }

    public MiDBContext(DbContextOptions<MiDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<People> People { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=workshop-testdb-glob.database.windows.net;Database=azure-workshop-testdb;User Id=testdb;Password=Testing123!;Encrypt=true;TrustServerCertificate=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<People>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
