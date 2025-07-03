using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Shared.Domain.Data.Config;

public partial class EfDbContext : DbContext
{
    public EfDbContext(DbContextOptions<EfDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Models.Users> Users { get; set; } = null!;

    public virtual DbSet<Models.Address> Addresses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
