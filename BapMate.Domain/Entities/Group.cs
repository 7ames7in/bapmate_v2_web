namespace BapMate.Domain.Entities;

public class Group
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public int MinMembers { get; set; }
    public int MaxMembers { get; set; }
    public string CostRule { get; set; } = string.Empty;
    public string Visibility { get; set; } = "public";
    public string HostId { get; set; } = string.Empty;
    public string? RestaurantId { get; set; }
    public string BillSplitMethod { get; set; } = "equal";
    public string? BillSplitRatiosJson { get; set; }
    public string? BillSplitGameId { get; set; }
    public string ParticipantsJson { get; set; } = "[]";
    public string ReviewsJson { get; set; } = "[]";
}
