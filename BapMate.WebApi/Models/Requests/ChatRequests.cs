namespace BapMate.WebApi.Models.Requests;

public record AddChatMessageRequest(
    string SenderId,
    string SenderName,
    string Content,
    string Timestamp);

public record EnsureChatThreadRequest(
    string GroupId,
    string? Title);
