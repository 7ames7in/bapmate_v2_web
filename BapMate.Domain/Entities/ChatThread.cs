namespace BapMate.Domain.Entities;

public class ChatThread
{
    public string Id { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string MessagesJson { get; set; } = "[]";
}
