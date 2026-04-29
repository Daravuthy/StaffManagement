using Microsoft.EntityFrameworkCore;
using StaffManagement.Api.Models;

namespace StaffManagement.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Staff> Staffs => Set<Staff>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Staff>(entity =>
        {
            entity.ToTable("Staffs");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.StaffId).IsUnique();
            entity.Property(x => x.StaffId).HasMaxLength(8).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Gender).IsRequired();
            entity.Property(x => x.Birthday).IsRequired();
        });
    }
}
