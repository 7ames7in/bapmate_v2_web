using System.Text.Json;

namespace BapMate.WebApi.Models;

public record FestivalProgramItemDto(
    string Time,
    string Title,
    string Description,
    string? Location);

public record FestivalContentDto(
    string Overview,
    IReadOnlyCollection<FestivalProgramItemDto> Program,
    IReadOnlyCollection<string> Highlights,
    IReadOnlyCollection<string> Rules,
    IReadOnlyCollection<string> Notices);

public record FestivalReviewDto(
    string Id,
    string UserId,
    string UserName,
    string UserAvatar,
    double Rating,
    string Comment,
    string Date,
    IReadOnlyCollection<string>? Images,
    int Helpful);

public record FestivalDto(
    string Id,
    string Title,
    string Description,
    string Location,
    string Address,
    double Lat,
    double Lng,
    string StartDate,
    string EndDate,
    string StartTime,
    string EndTime,
    string Category,
    string Organizer,
    string ContactInfo,
    IReadOnlyCollection<string> Images,
    IReadOnlyCollection<string> Tags,
    int ParticipantCount,
    int? MaxParticipants,
    int Likes,
    bool IsLiked,
    decimal? TicketPrice,
    bool IsBookingRequired,
    string? BookingUrl,
    IReadOnlyCollection<string> Facilities,
    IReadOnlyCollection<string> Accessibility,
    bool Parking,
    IReadOnlyCollection<string> PublicTransport,
    string? Website,
    JsonElement? SocialMedia,
    FestivalContentDto Content,
    JsonElement? Weather,
    IReadOnlyCollection<string>? NearbyRestaurants,
    IReadOnlyCollection<FestivalReviewDto> Reviews);
