using Microsoft.EntityFrameworkCore;
using BapMate.Domain.Entities;

namespace BapMate.Infrastructure.Data;

public class BapMateDbContext : DbContext
{
    public BapMateDbContext(DbContextOptions<BapMateDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Friend> Friends => Set<Friend>();
    public DbSet<GameRoom> GameRooms => Set<GameRoom>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("Users").Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<Friend>().ToTable("Friends").Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<GameRoom>().ToTable("GameRooms").Property(e => e.Id).ValueGeneratedNever();
    }
}
