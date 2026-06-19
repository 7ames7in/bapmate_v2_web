namespace BapMate.WebApi.Models;

public record MenuItemReviewDto(
    string Id,
    string UserId,
    string UserName,
    string? UserAvatar,
    double Rating,
    string Comment,
    string Date,
    int Helpful,
    IReadOnlyCollection<string>? Images);

public record MenuItemReviewerDto(
    string ReviewId,
    string UserId,
    string UserName,
    string? UserAvatar);

public record MenuItemDto(
    string Id,
    string Name,
    string? Description,
    decimal Price,
    string Category,
    string? Image,
    double? Rating,
    bool? Popular,
    IReadOnlyCollection<MenuItemReviewDto>? Reviews,
    int? TotalReviews,
    IReadOnlyCollection<MenuItemReviewerDto>? ReviewerProfiles,
    IReadOnlyCollection<string>? Tags);

public record RestaurantCouponDto(
    string Id,
    string Title,
    string Condition,
    string ExpiresAt);

public record RestaurantEventDto(
    string Id,
    string Type,
    string Title,
    string Date,
    int ParticipantCount,
    string EventId,
    string? Description,
    decimal? TotalCost,
    string? PaymentMethod);

public record RestaurantDto(
    string Id,
    string Name,
    string Category,
    double Rating,
    string Distance,
    string Address,
    double Lat,
    double Lng,
    IReadOnlyCollection<MenuItemDto> Menu,
    IReadOnlyCollection<RestaurantReviewDto> Reviews,
    IReadOnlyCollection<RestaurantCouponDto> Coupons,
    IReadOnlyCollection<RestaurantEventDto>? EventHistory,
    int? TotalReviews,
    string? PriceRange,
    string? BusinessHours,
    string? Phone);

public record RestaurantReferenceDto(
    string Id,
    string Name,
    string Address,
    string Category,
    string City,
    IReadOnlyCollection<string> Tags);
