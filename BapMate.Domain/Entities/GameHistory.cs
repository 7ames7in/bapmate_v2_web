namespace BapMate.Domain.Entities;

public class GameHistory
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public string ParticipantsJson { get; set; } = "[]";
    public string? Winner { get; set; }
    public string Mode { get; set; } = "direct";
    public int? MaxParticipants { get; set; }
    public string? Description { get; set; }
    public string? TagsJson { get; set; }
    public string? Wager { get; set; }
    public string Status { get; set; } = "draft";
    public string? HostId { get; set; }
    public string? SeriesJson { get; set; }
    public string? RankingJson { get; set; }
    public string? LottoJson { get; set; }
    public string? GameMode { get; set; }
    public string? GameResultJson { get; set; }
    public string? PredeterminedWinner { get; set; }
    public string? PredeterminedReplayDataJson { get; set; }
    public string? GroupId { get; set; }
    public string? GroupTitle { get; set; }
}
