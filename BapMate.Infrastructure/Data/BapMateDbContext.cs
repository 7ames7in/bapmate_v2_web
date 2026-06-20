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
        
        modelBuilder.Entity<GameRoom>(entity =>
        {
            entity.ToTable("gamerooms");
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(e => e.HostName).HasColumnName("hostname");
            entity.Property(e => e.SettingsJson).HasColumnName("settingsjson");
            entity.Property(e => e.PlayersJson).HasColumnName("playersjson");
            entity.Property(e => e.IsStarted).HasColumnName("isstarted");
            entity.Property(e => e.IsEnded).HasColumnName("isended");
            entity.Property(e => e.CreatedAt).HasColumnName("createdat");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedat");
        });
    }
}
