using System;

namespace BapMate.Domain.Entities;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string Avatar { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Carrier { get; set; } = string.Empty;
    public double ReliabilityScore { get; set; }
    public decimal WalletBalance { get; set; }
    public decimal EscrowBalance { get; set; }
    public string BadgesJson { get; set; } = "[]";
    public string MatchPreferencesJson { get; set; } = "{}";
    public string DefaultGameSettingsJson { get; set; } = "{}";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Password { get; set; } = string.Empty;
}
