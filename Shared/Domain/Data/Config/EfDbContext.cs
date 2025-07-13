using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Data.Models;

namespace Shared.Domain.Data.Config;

public partial class EfDbContext : DbContext
{
    public EfDbContext(DbContextOptions<EfDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Users> Users { get; set; } = null!;

    public virtual DbSet<Address> Addresses { get; set; } = null!;

    public virtual DbSet<Wallet> Wallets { get; set; } = null!;

    public virtual DbSet<WalletTransaction> WalletTransactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Wallet>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<WalletTransaction>(entity =>
        {
            entity.HasOne(wt => wt.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(wt => wt.WalletId)
                .OnDelete(DeleteBehavior.NoAction);

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
