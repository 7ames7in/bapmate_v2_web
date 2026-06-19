namespace BapMate.WebApi.Models;

public record ChatMessageDto(
    string Id,
    string SenderId,
    string SenderName,
    string Content,
    string Timestamp);

public record ChatThreadDto(
    string Id,
    string GroupId,
    string Title,
    IReadOnlyCollection<ChatMessageDto> Messages);
