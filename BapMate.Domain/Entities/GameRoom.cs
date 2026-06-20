using System;

namespace BapMate.Domain.Entities;

public class GameRoom
{
    public string Id { get; set; } = string.Empty; // RoomId
    public string HostName { get; set; } = string.Empty;
    public string SettingsJson { get; set; } = "{}";
    public string PlayersJson { get; set; } = "[]";
    public bool IsStarted { get; set; }
    public bool IsEnded { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
