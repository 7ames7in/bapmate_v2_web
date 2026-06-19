namespace BapMate.WebApi.Models;

public record WalletSummaryDto(
    string UserId,
    decimal WalletBalance,
    decimal EscrowBalance,
    decimal AvailableToWithdraw,
    decimal PendingEscrow,
    decimal RecentlySpent,
    decimal RewardsEarnedThisMonth,
    string UpdatedAt);

public record PaymentTransactionDto(
    string Id,
    string UserId,
    string Title,
    string Category,
    string Type,
    string Direction,
    decimal Amount,
    decimal WalletDelta,
    decimal EscrowDelta,
    decimal WalletBalanceAfter,
    decimal EscrowBalanceAfter,
    string Currency,
    string? Counterparty,
    string? ReferenceId,
    string? ReferenceType,
    string? Memo,
    string CreatedAt);

public record PayActionResponse(
    WalletSummaryDto Summary,
    PaymentTransactionDto Transaction);
