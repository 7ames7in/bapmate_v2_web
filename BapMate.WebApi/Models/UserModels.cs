namespace BapMate.WebApi.Models;

public record MatchPreferencesDto(
    IReadOnlyCollection<string> PreferredTimeSlots,
    IReadOnlyCollection<string> PreferredPaymentTypes,
    IReadOnlyCollection<string> PreferredInterests);

public record DefaultGameSettingsDto(
    IReadOnlyCollection<string> DefaultMissions,
    IReadOnlyCollection<string> DefaultCosts);

public record UserProfileDto(
    string Id,
    string Name,
    string Email,
    string Avatar,
    string Bio,
    double ReliabilityScore,
    decimal WalletBalance,
    decimal EscrowBalance,
    IReadOnlyCollection<string> Badges,
    MatchPreferencesDto MatchPreferences,
    DefaultGameSettingsDto DefaultGameSettings,
    string Gender,
    int BirthYear,
    string? Phone);

public record FriendDto(
    string Id,
    string OwnerId,
    string Name,
    string Avatar,
    int TrustLevel,
    string LastMeal,
    IReadOnlyCollection<string> Tags,
    string? Memo,
    string? Phone,
    string? Identifier);

public record SupportRequestDto(
    string Id,
    string Title,
    string Story,
    decimal Amount,
    bool Verified,
    double Progress,
    string ExpiresAt);
