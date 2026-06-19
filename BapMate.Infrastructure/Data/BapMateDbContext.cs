using Microsoft.EntityFrameworkCore;         // ✅ EF Core 타입들
using BapMate.Domain.Entities;

namespace BapMate.Infrastructure.Data;        // ✅ Program.cs의 using과 일치

public class BapMateDbContext : DbContext
{
    public BapMateDbContext(DbContextOptions<BapMateDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().Property(e => e.Id).ValueGeneratedNever();
    }
}
