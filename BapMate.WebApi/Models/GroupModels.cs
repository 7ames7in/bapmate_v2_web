namespace BapMate.WebApi.Models;

public record ParticipantDto(
    string Id,
    string Name,
    string Avatar,
    string Status,
    string? Intention,
    int? LikabilityScore);

public record RestaurantReviewDto(
    string Id,
    string UserId,
    string UserName,
    string? UserAvatar,
    double Rating,
    string Comment,
    string Date,
    int Helpful,
    IReadOnlyCollection<string>? Images,
    string? VisitedWithGroup,
    IReadOnlyCollection<string>? OrderItems);

public record BillSplitDto(
    string Method,
    IReadOnlyDictionary<string, double>? Ratios,
    string? GameId);

public record GroupDto(
    string Id,
    string Title,
    string Description,
    string Location,
    string Time,
    string CreatedAt,
    int MinMembers,
    int MaxMembers,
    string CostRule,
    IReadOnlyCollection<ParticipantDto> Participants,
    IReadOnlyCollection<RestaurantReviewDto> Reviews,
    string Visibility,
    string HostId,
    BillSplitDto BillSplit,
    string? RestaurantId);
