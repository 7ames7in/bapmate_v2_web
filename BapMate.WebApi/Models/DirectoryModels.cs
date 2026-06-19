namespace BapMate.WebApi.Models;

public record UserDirectoryEntryDto(
    string Id,
    string Name,
    string Avatar,
    string Phone,
    string Identifier,
    string? Bio);
