namespace BapMate.WebApi.Models.Requests;

public record AddMatchSuggestionRequest(
    string AuthorId,
    string AuthorName,
    string Message,
    string? RestaurantIdea,
    string? RestaurantId,
    string CreatedAt);
