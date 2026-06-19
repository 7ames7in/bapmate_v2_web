namespace BapMate.WebApi.Models.Requests;

public record ParticipantRequest(
    string Id,
    string Name,
    string Avatar,
    string Status,
    string? Intention,
    int? LikabilityScore);

public record GroupReviewRequest(
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

public record UpsertGroupRequest(
    string Title,
    string Description,
    string Location,
    string Time,
    string CreatedAt,
    int MinMembers,
    int MaxMembers,
    string CostRule,
    string Visibility,
    string HostId,
    string BillSplitMethod,
    IReadOnlyDictionary<string, double>? BillSplitRatios,
    string? BillSplitGameId,
    string? RestaurantId,
    IReadOnlyCollection<ParticipantRequest> Participants,
    IReadOnlyCollection<GroupReviewRequest> Reviews);

public record JoinGroupRequest(
    ParticipantRequest Participant,
    string? Status,
    string? Intention);

public record UpdateParticipantStatusRequest(string Status);
