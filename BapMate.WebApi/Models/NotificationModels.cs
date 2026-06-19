namespace BapMate.WebApi.Models;

public record NotificationDto(
    string Id,
    string Type,
    string Title,
    string Message,
    string Time);
