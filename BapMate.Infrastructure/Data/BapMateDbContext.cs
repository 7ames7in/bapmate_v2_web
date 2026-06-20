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

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.PasswordHash).HasColumnName("passwordhash");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.BirthYear).HasColumnName("birthyear");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.Carrier).HasColumnName("carrier");
            entity.Property(e => e.ReliabilityScore).HasColumnName("reliabilityscore");
            entity.Property(e => e.WalletBalance).HasColumnName("walletbalance");
            entity.Property(e => e.EscrowBalance).HasColumnName("escrowbalance");
            entity.Property(e => e.BadgesJson).HasColumnName("badgesjson");
            entity.Property(e => e.MatchPreferencesJson).HasColumnName("matchpreferencesjson");
            entity.Property(e => e.DefaultGameSettingsJson).HasColumnName("defaultgamesettingsjson");
            entity.Property(e => e.CreatedAt).HasColumnName("createdat");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedat");
            entity.Property(e => e.Password).HasColumnName("password");
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.ToTable("friends");
            entity.HasKey(f => new { f.OwnerId, f.Id });
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(e => e.OwnerId).HasColumnName("ownerid");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.TrustLevel).HasColumnName("trustlevel");
            entity.Property(e => e.LastMeal).HasColumnName("lastmeal");
            entity.Property(e => e.TagsJson).HasColumnName("tagsjson");
            entity.Property(e => e.Memo).HasColumnName("memo");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.Identifier).HasColumnName("identifier");
        });
        
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
