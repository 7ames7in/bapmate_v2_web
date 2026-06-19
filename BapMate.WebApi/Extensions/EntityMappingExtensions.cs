using BapMate.Domain.Entities;
using BapMate.WebApi.Infrastructure;
using BapMate.WebApi.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace BapMate.WebApi.Extensions;

public static class EntityMappingExtensions
{
    private static readonly MatchPreferencesPayload EmptyMatchPreferences = new([], [], []);
    private static readonly DefaultGameSettingsPayload EmptyGameSettings = new([], []);

    public static UserProfileDto ToUserProfileDto(this User entity)
    {
        var badges = JsonContentHelper.DeserializeList<string>(entity.BadgesJson);
        var matchPreferences = JsonContentHelper.DeserializeObject<MatchPreferencesPayload>(entity.MatchPreferencesJson) ?? EmptyMatchPreferences;
        var defaultSettings = JsonContentHelper.DeserializeObject<DefaultGameSettingsPayload>(entity.DefaultGameSettingsJson) ?? EmptyGameSettings;

        return new UserProfileDto(
            entity.Id,
            entity.Name,
            entity.Email,
            entity.Avatar,
            entity.Bio,
            entity.ReliabilityScore,
            entity.WalletBalance,
            entity.EscrowBalance,
            badges,
            new MatchPreferencesDto(matchPreferences.PreferredTimeSlots, matchPreferences.PreferredPaymentTypes, matchPreferences.PreferredInterests),
            new DefaultGameSettingsDto(defaultSettings.DefaultMissions, defaultSettings.DefaultCosts),
            entity.Gender,
            entity.BirthYear,
            entity.Phone);
    }

    public static FriendDto ToFriendDto(this Friend entity)
    {
        var tags = JsonContentHelper.DeserializeList<string>(entity.TagsJson);
        return new FriendDto(
            entity.Id,
            entity.OwnerId,
            entity.Name,
            entity.Avatar,
            entity.TrustLevel,
            entity.LastMeal,
            tags,
            entity.Memo,
            entity.Phone,
            entity.Identifier);
    }

    public static SupportRequestDto ToSupportRequestDto(this SupportRequest entity) =>
        new(entity.Id, entity.Title, entity.Story, entity.Amount, entity.Verified, entity.Progress, entity.ExpiresAt);

    public static GroupDto ToGroupDto(this Group entity)
    {
        var participants = JsonContentHelper
            .DeserializeList<ParticipantPayload>(entity.ParticipantsJson)
            .Select(p => new ParticipantDto(p.Id, p.Name, p.Avatar, p.Status, p.Intention, p.LikabilityScore))
            .ToArray();

        var reviews = JsonContentHelper
            .DeserializeList<RestaurantReviewPayload>(entity.ReviewsJson)
            .Select(r => new RestaurantReviewDto(
                r.Id,
                r.UserId,
                r.UserName,
                r.UserAvatar,
                r.Rating,
                r.Comment,
                r.Date,
                r.Helpful,
                r.Images,
                r.VisitedWithGroup,
                r.OrderItems))
            .ToArray();

        var ratios = entity.BillSplitRatiosJson is null
            ? null
            : JsonContentHelper.DeserializeDictionary<string, double>(entity.BillSplitRatiosJson);

        return new GroupDto(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.Location,
            entity.Time,
            entity.CreatedAt,
            entity.MinMembers,
            entity.MaxMembers,
            entity.CostRule,
            participants,
            reviews,
            entity.Visibility,
            entity.HostId,
            new BillSplitDto(
                entity.BillSplitMethod,
                ratios?.Count > 0 ? ratios : null,
                entity.BillSplitGameId),
            entity.RestaurantId);
    }

    public static RestaurantDto ToRestaurantDto(this Restaurant entity)
    {
        var menu = JsonContentHelper
            .DeserializeList<MenuItemPayload>(entity.MenuJson)
            .Select(ToMenuItemDto)
            .ToArray();

        var reviews = JsonContentHelper
            .DeserializeList<RestaurantReviewPayload>(entity.ReviewsJson)
            .Select(r => new RestaurantReviewDto(
                r.Id,
                r.UserId,
                r.UserName,
                r.UserAvatar,
                r.Rating,
                r.Comment,
                r.Date,
                r.Helpful,
                r.Images,
                r.VisitedWithGroup,
                r.OrderItems))
            .ToArray();

        var coupons = JsonContentHelper
            .DeserializeList<RestaurantCouponPayload>(entity.CouponsJson)
            .Select(c => new RestaurantCouponDto(c.Id, c.Title, c.Condition, c.ExpiresAt))
            .ToArray();

        var events = JsonContentHelper
            .DeserializeList<RestaurantEventPayload>(entity.EventHistoryJson)
            .Select(e => new RestaurantEventDto(
                e.Id,
                e.Type,
                e.Title,
                e.Date,
                e.ParticipantCount,
                e.EventId,
                e.Description,
                e.TotalCost,
                e.PaymentMethod))
            .ToArray();

        return new RestaurantDto(
            entity.Id,
            entity.Name,
            entity.Category,
            entity.Rating,
            entity.Distance,
            entity.Address,
            entity.Latitude,
            entity.Longitude,
            menu,
            reviews,
            coupons,
            events.Length > 0 ? events : null,
            entity.TotalReviews,
            entity.PriceRange,
            entity.BusinessHours,
            entity.Phone);
    }

    public static PaymentTransactionDto ToPaymentTransactionDto(this PaymentTransaction entity) =>
        new(
            entity.Id,
            entity.UserId,
            entity.Title,
            entity.Category,
            entity.Type,
            entity.Direction,
            entity.Amount,
            entity.WalletDelta,
            entity.EscrowDelta,
            entity.WalletBalanceAfter,
            entity.EscrowBalanceAfter,
            entity.Currency,
            entity.Counterparty,
            entity.ReferenceId,
            entity.ReferenceType,
            entity.Memo,
            entity.CreatedAt.ToString("o", CultureInfo.InvariantCulture));

    public static RestaurantReferenceDto ToRestaurantReferenceDto(this RestaurantReference entity)
    {
        var tags = JsonContentHelper.DeserializeList<string>(entity.TagsJson);
        return new RestaurantReferenceDto(entity.Id, entity.Name, entity.Address, entity.Category, entity.City, tags);
    }

    public static NotificationDto ToNotificationDto(this Notification entity) =>
        new(entity.Id, entity.Type, entity.Title, entity.Message, entity.Time);

    public static ChatThreadDto ToChatThreadDto(this ChatThread entity)
    {
        var messages = JsonContentHelper
            .DeserializeList<ChatMessagePayload>(entity.MessagesJson)
            .Select(m => new ChatMessageDto(m.Id, m.SenderId, m.SenderName, m.Content, m.Timestamp))
            .ToArray();

        return new ChatThreadDto(entity.Id, entity.GroupId, entity.Title, messages);
    }

    public static GameHistoryDto ToGameHistoryDto(this GameHistory entity)
    {
        var participants = JsonContentHelper.DeserializeList<string>(entity.ParticipantsJson);
        var tags = entity.TagsJson is null ? null : JsonContentHelper.DeserializeList<string>(entity.TagsJson);
        var wager = JsonContentHelper.DeserializeElement(entity.Wager);
        var series = JsonContentHelper.DeserializeObject<GameSeriesPayload>(entity.SeriesJson);
        var ranking = JsonContentHelper.DeserializeObject<RankingGamePayload>(entity.RankingJson);
        var lotto = JsonContentHelper.DeserializeObject<LottoGamePayload>(entity.LottoJson);
        var gameResult = JsonContentHelper.DeserializeObject<GameResultPayload>(entity.GameResultJson);
        var predeterminedReplay = JsonContentHelper.DeserializeElement(entity.PredeterminedReplayDataJson);

        return new GameHistoryDto(
            entity.Id,
            entity.Type,
            entity.CreatedAt,
            participants,
            entity.Winner,
            entity.Mode,
            entity.MaxParticipants,
            entity.Description,
            tags?.Count > 0 ? tags : null,
            wager,
            entity.Status,
            entity.HostId,
            series?.ToDto(),
            ranking?.ToDto(),
            lotto?.ToDto(),
            entity.GameMode,
            gameResult?.ToDto(),
            entity.PredeterminedWinner,
            predeterminedReplay,
            entity.GroupId,
            entity.GroupTitle);
    }

    public static GameHistory ToEntity(this GameHistoryDto dto, string? idOverride = null)
    {
        var id = string.IsNullOrWhiteSpace(idOverride)
            ? (string.IsNullOrWhiteSpace(dto.Id) ? $"game-{Guid.NewGuid():N}" : dto.Id)
            : idOverride;

        var createdAt = string.IsNullOrWhiteSpace(dto.CreatedAt)
            ? DateTime.UtcNow.ToString("O")
            : dto.CreatedAt;

        return new GameHistory
        {
            Id = id,
            Type = dto.Type,
            CreatedAt = createdAt,
            ParticipantsJson = JsonContentHelper.Serialize(dto.Participants),
            Winner = dto.Winner,
            Mode = dto.Mode,
            MaxParticipants = dto.MaxParticipants,
            Description = dto.Description,
            TagsJson = dto.Tags is { Count: > 0 } ? JsonContentHelper.Serialize(dto.Tags) : null,
            Wager = JsonContentHelper.SerializeElement(dto.Wager),
            Status = dto.Status,
            HostId = dto.HostId,
            SeriesJson = dto.Series is null ? null : JsonContentHelper.Serialize(dto.Series),
            RankingJson = dto.Ranking is null ? null : JsonContentHelper.Serialize(dto.Ranking),
            LottoJson = dto.Lotto is null ? null : JsonContentHelper.Serialize(dto.Lotto),
            GameMode = dto.GameMode,
            GameResultJson = dto.GameResult is null ? null : JsonContentHelper.Serialize(dto.GameResult),
            PredeterminedWinner = dto.PredeterminedWinner,
            PredeterminedReplayDataJson = JsonContentHelper.SerializeElement(dto.PredeterminedReplayData),
            GroupId = dto.GroupId,
            GroupTitle = dto.GroupTitle,
        };
    }

    public static FestivalDto ToFestivalDto(this Festival entity)
    {
        var images = JsonContentHelper.DeserializeList<string>(entity.ImagesJson);
        var tags = JsonContentHelper.DeserializeList<string>(entity.TagsJson);
        var facilities = JsonContentHelper.DeserializeList<string>(entity.FacilitiesJson);
        var accessibility = JsonContentHelper.DeserializeList<string>(entity.AccessibilityJson);
        var publicTransport = JsonContentHelper.DeserializeList<string>(entity.PublicTransportJson);
        var socialMedia = JsonContentHelper.DeserializeElement(entity.SocialMediaJson);
        var weather = JsonContentHelper.DeserializeElement(entity.WeatherJson);
        var nearbyRestaurants = entity.NearbyRestaurantsJson is null ? null : JsonContentHelper.DeserializeList<string>(entity.NearbyRestaurantsJson);
        var reviews = JsonContentHelper
            .DeserializeList<FestivalReviewPayload>(entity.ReviewsJson)
            .Select(r => new FestivalReviewDto(r.Id, r.UserId, r.UserName, r.UserAvatar, r.Rating, r.Comment, r.Date, r.Images, r.Helpful))
            .ToArray();

        var contentPayload = JsonContentHelper.DeserializeObject<FestivalContentPayload>(entity.ContentJson) ?? FestivalContentPayload.Empty;

        return new FestivalDto(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.Location,
            entity.Address,
            entity.Latitude,
            entity.Longitude,
            entity.StartDate,
            entity.EndDate,
            entity.StartTime,
            entity.EndTime,
            entity.Category,
            entity.Organizer,
            entity.ContactInfo,
            images,
            tags,
            entity.ParticipantCount,
            entity.MaxParticipants,
            entity.Likes,
            entity.IsLiked,
            entity.TicketPrice,
            entity.IsBookingRequired,
            entity.BookingUrl,
            facilities,
            accessibility,
            entity.Parking,
            publicTransport,
            entity.Website,
            socialMedia,
            contentPayload.ToDto(),
            weather,
            nearbyRestaurants?.Count > 0 ? nearbyRestaurants : null,
            reviews);
    }

    public static MatchRequestDto ToMatchRequestDto(this MatchRequest entity)
    {
        var interests = JsonContentHelper.DeserializeList<string>(entity.InterestsJson);
        var suggestions = JsonContentHelper
            .DeserializeList<MatchRequestSuggestionPayload>(entity.SuggestionsJson)
            .Select(s => new MatchRequestSuggestionDto(s.Id, s.AuthorId, s.AuthorName, s.Message, s.RestaurantIdea, s.RestaurantId, s.CreatedAt))
            .ToArray();

        return new MatchRequestDto(
            entity.Id,
            entity.Title,
            entity.Message,
            entity.Location,
            entity.RadiusKm,
            entity.TimeSlot,
            entity.PreferredAt,
            entity.PartySize,
            entity.GenderPreference,
            entity.AgeRange,
            interests,
            entity.DepositRequired,
            entity.Status,
            entity.CreatedAt,
            entity.CreatedBy,
            entity.PaymentType,
            entity.ConfirmedWith,
            entity.Notes,
            entity.RestaurantId,
            entity.AllowRestaurantSuggestions,
            suggestions);
    }

    public static UserDirectoryEntryDto ToDirectoryEntryDto(this UserDirectoryEntry entity) =>
        new(entity.Id, entity.Name, entity.Avatar, entity.Phone, entity.Identifier, entity.Bio);

    #region Payloads

    private sealed record MatchPreferencesPayload(
        IReadOnlyCollection<string> PreferredTimeSlots,
        IReadOnlyCollection<string> PreferredPaymentTypes,
        IReadOnlyCollection<string> PreferredInterests);

    private sealed record DefaultGameSettingsPayload(
        IReadOnlyCollection<string> DefaultMissions,
        IReadOnlyCollection<string> DefaultCosts);

    private sealed record ParticipantPayload(
        string Id,
        string Name,
        string Avatar,
        string Status,
        string? Intention,
        int? LikabilityScore);

    private sealed record RestaurantReviewPayload(
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

    private sealed record MenuItemReviewPayload(
        string Id,
        string UserId,
        string UserName,
        string? UserAvatar,
        double Rating,
        string Comment,
        string Date,
        int Helpful,
        IReadOnlyCollection<string>? Images);

    private sealed record MenuItemPayload(
        string Id,
        string Name,
        string? Description,
        decimal Price,
        string Category,
        string? Image,
        double? Rating,
        bool? Popular,
        IReadOnlyCollection<MenuItemReviewPayload>? Reviews,
        int? TotalReviews,
        IReadOnlyCollection<string>? Tags);

    private sealed record RestaurantCouponPayload(
        string Id,
        string Title,
        string Condition,
        string ExpiresAt);

    private sealed record RestaurantEventPayload(
        string Id,
        string Type,
        string Title,
        string Date,
        int ParticipantCount,
        string EventId,
        string? Description,
        decimal? TotalCost,
        string? PaymentMethod);

    private sealed record ChatMessagePayload(
        string Id,
        string SenderId,
        string SenderName,
        string Content,
        string Timestamp);

    private sealed record GameSeriesPayload(
        int MaxRounds,
        int RequiredWins,
        IReadOnlyCollection<GameRoundPayload> Rounds,
        int CurrentRound,
        bool IsCompleted,
        string? FinalWinner,
        IReadOnlyDictionary<string, int> WinCounts,
        IReadOnlyDictionary<string, double> WinRates)
    {
        public GameSeriesDto ToDto() =>
            new(
                MaxRounds,
                RequiredWins,
                Rounds.Select(r => new GameRoundDto(r.RoundNumber, r.Participants, r.Results, r.Winner, r.PlayedAt)).ToArray(),
                CurrentRound,
                IsCompleted,
                FinalWinner,
                WinCounts,
                WinRates);
    }

    private sealed record GameRoundPayload(
        int RoundNumber,
        IReadOnlyCollection<string> Participants,
        IReadOnlyDictionary<string, double> Results,
        string? Winner,
        string PlayedAt);

    private sealed record RankingGamePayload(
        int CurrentRound,
        int TotalRounds,
        IReadOnlyCollection<string> Rankings,
        IReadOnlyCollection<string> RemainingParticipants,
        bool IsCompleted,
        IReadOnlyCollection<RankingRoundPayload> RoundResults)
    {
        public RankingGameDto ToDto() =>
            new(
                CurrentRound,
                TotalRounds,
                Rankings,
                RemainingParticipants,
                IsCompleted,
                RoundResults.Select(r => new RankingGameRoundResultDto(r.Round, r.Participants, r.Winner, r.Timestamp, r.ReplayData)).ToArray());
    }

    private sealed record RankingRoundPayload(
        int Round,
        IReadOnlyCollection<string> Participants,
        string Winner,
        string Timestamp,
        JsonElement? ReplayData);

    private sealed record LottoGamePayload(
        IReadOnlyCollection<LottoDrawRecordPayload> Draws,
        bool IsCompleted)
    {
        public LottoGameDto ToDto() =>
            new(Draws.Select(d => new LottoDrawRecordDto(d.Number, d.Name, d.Order, d.DrawnAt)).ToArray(), IsCompleted);
    }

    private sealed record LottoDrawRecordPayload(
        int Number,
        string? Name,
        int Order,
        string DrawnAt);

    private sealed record GameResultPayload(
        string Type,
        string Winner,
        IReadOnlyCollection<string> Participants,
        JsonElement? Details,
        string Timestamp,
        JsonElement? ReplayData)
    {
        public GameResultDto ToDto() =>
            new(
                Type,
                Winner,
                Participants,
                Details,
                Timestamp,
                ReplayData);
    }

    private sealed record FestivalReviewPayload(
        string Id,
        string UserId,
        string UserName,
        string UserAvatar,
        double Rating,
        string Comment,
        string Date,
        IReadOnlyCollection<string>? Images,
        int Helpful);

    private sealed record FestivalContentPayload(
        string Overview,
        IReadOnlyCollection<FestivalProgramItemPayload> Program,
        IReadOnlyCollection<string> Highlights,
        IReadOnlyCollection<string> Rules,
        IReadOnlyCollection<string> Notices)
    {
        public static FestivalContentPayload Empty { get; } = new(
            string.Empty,
            Array.Empty<FestivalProgramItemPayload>(),
            Array.Empty<string>(),
            Array.Empty<string>(),
            Array.Empty<string>());

        public FestivalContentDto ToDto() =>
            new(
                Overview,
                Program.Select(p => new FestivalProgramItemDto(p.Time, p.Title, p.Description, p.Location)).ToArray(),
                Highlights,
                Rules,
                Notices);
    }

    private sealed record FestivalProgramItemPayload(
        string Time,
        string Title,
        string Description,
        string? Location);

    private sealed record MatchRequestSuggestionPayload(
        string Id,
        string AuthorId,
        string AuthorName,
        string Message,
        string? RestaurantIdea,
        string? RestaurantId,
        string CreatedAt);

    #endregion

    #region Helpers

    private static MenuItemDto ToMenuItemDto(MenuItemPayload payload)
    {
        var reviews = payload.Reviews?.Select(r => new MenuItemReviewDto(r.Id, r.UserId, r.UserName, r.UserAvatar, r.Rating, r.Comment, r.Date, r.Helpful, r.Images)).ToArray();
        var reviewerProfiles = payload.Reviews?
            .Take(3)
            .Select(r => new MenuItemReviewerDto(r.Id, r.UserId, r.UserName, r.UserAvatar))
            .ToArray();
        return new MenuItemDto(
            payload.Id,
            payload.Name,
            payload.Description,
            payload.Price,
            payload.Category,
            payload.Image,
            payload.Rating,
            payload.Popular,
            reviews,
            payload.TotalReviews,
            reviewerProfiles,
            payload.Tags);
    }

    #endregion
}
