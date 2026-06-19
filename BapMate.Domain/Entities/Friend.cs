namespace BapMate.Domain.Entities;

public class Friend
{
    public string Id { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public int TrustLevel { get; set; }
    public string LastMeal { get; set; } = string.Empty;
    public string TagsJson { get; set; } = "[]";
    public string? Memo { get; set; }
    public string? Phone { get; set; }
    public string? Identifier { get; set; }
}
