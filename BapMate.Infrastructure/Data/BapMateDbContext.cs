using Microsoft.EntityFrameworkCore;         // ✅ EF Core 타입들
using BapMate.Domain.Entities;

namespace BapMate.Infrastructure.Data;        // ✅ Program.cs의 using과 일치

public class BapMateDbContext : DbContext
{
    public BapMateDbContext(DbContextOptions<BapMateDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Friend> Friends => Set<Friend>();
    public DbSet<SupportRequest> SupportRequests => Set<SupportRequest>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ChatThread> ChatThreads => Set<ChatThread>();
    public DbSet<GameHistory> GameHistories => Set<GameHistory>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<Festival> Festivals => Set<Festival>();
    public DbSet<MatchRequest> MatchRequests => Set<MatchRequest>();
    public DbSet<UserDirectoryEntry> UserDirectory => Set<UserDirectoryEntry>();
    public DbSet<RestaurantReference> RestaurantDirectory => Set<RestaurantReference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<Friend>().Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<SupportRequest>().Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<Group>().Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<Restaurant>().Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<Notification>().Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<ChatThread>().Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<GameHistory>().Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<PaymentTransaction>().Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<Festival>().Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<MatchRequest>().Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<UserDirectoryEntry>().Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<RestaurantReference>().Property(e => e.Id).ValueGeneratedNever();
    }
}
