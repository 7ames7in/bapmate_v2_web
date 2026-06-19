namespace BapMate.WebApi.Models;

public record MatchRequestSuggestionDto(
    string Id,
    string AuthorId,
    string AuthorName,
    string Message,
    string? RestaurantIdea,
    string? RestaurantId,
    string CreatedAt);

public record MatchRequestDto(
    string Id,
    string Title,
    string Message,
    string Location,
    double RadiusKm,
    string TimeSlot,
    string PreferredAt,
    int PartySize,
    string GenderPreference,
    string? AgeRange,
    IReadOnlyCollection<string> Interests,
    bool DepositRequired,
    string Status,
    string CreatedAt,
    string CreatedBy,
    string PaymentType,
    string? ConfirmedWith,
    string? Notes,
    string? RestaurantId,
    bool AllowRestaurantSuggestions,
    IReadOnlyCollection<MatchRequestSuggestionDto> Suggestions);
