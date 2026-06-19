using System.Text.Json;

namespace BapMate.WebApi.Models;

public record GameRoundDto(
    int RoundNumber,
    IReadOnlyCollection<string> Participants,
    IReadOnlyDictionary<string, double> Results,
    string? Winner,
    string PlayedAt);

public record GameSeriesDto(
    int MaxRounds,
    int RequiredWins,
    IReadOnlyCollection<GameRoundDto> Rounds,
    int CurrentRound,
    bool IsCompleted,
    string? FinalWinner,
    IReadOnlyDictionary<string, int> WinCounts,
    IReadOnlyDictionary<string, double> WinRates);

public record RankingGameRoundResultDto(
    int Round,
    IReadOnlyCollection<string> Participants,
    string Winner,
    string Timestamp,
    JsonElement? ReplayData);

public record RankingGameDto(
    int CurrentRound,
    int TotalRounds,
    IReadOnlyCollection<string> Rankings,
    IReadOnlyCollection<string> RemainingParticipants,
    bool IsCompleted,
    IReadOnlyCollection<RankingGameRoundResultDto> RoundResults);

public record LottoDrawRecordDto(
    int Number,
    string? Name,
    int Order,
    string DrawnAt);

public record LottoReplayDataDto(
    IReadOnlyCollection<LottoDrawRecordDto> Draws,
    string? CompletedAt);

public record LottoGameDto(
    IReadOnlyCollection<LottoDrawRecordDto> Draws,
    bool IsCompleted);

public record GameResultDto(
    string Type,
    string Winner,
    IReadOnlyCollection<string> Participants,
    JsonElement? Details,
    string Timestamp,
    JsonElement? ReplayData);

public record GameHistoryDto(
    string Id,
    string Type,
    string CreatedAt,
    IReadOnlyCollection<string> Participants,
    string? Winner,
    string Mode,
    int? MaxParticipants,
    string? Description,
    IReadOnlyCollection<string>? Tags,
    JsonElement? Wager,
    string Status,
    string? HostId,
    GameSeriesDto? Series,
    RankingGameDto? Ranking,
    LottoGameDto? Lotto,
    string? GameMode,
    GameResultDto? GameResult,
    string? PredeterminedWinner,
    JsonElement? PredeterminedReplayData,
    string? GroupId,
    string? GroupTitle);
