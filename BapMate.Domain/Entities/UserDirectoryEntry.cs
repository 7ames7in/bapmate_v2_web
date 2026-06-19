namespace BapMate.Domain.Entities;

public class UserDirectoryEntry
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public string? Bio { get; set; }
}
