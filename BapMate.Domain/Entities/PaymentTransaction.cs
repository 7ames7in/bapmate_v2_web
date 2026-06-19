using System;

namespace BapMate.Domain.Entities;

public class PaymentTransaction
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = "general";
    public string Type { get; set; } = "topup";
    public string Direction { get; set; } = "credit"; // credit, debit, transfer
    public decimal Amount { get; set; }
    public decimal WalletDelta { get; set; }
    public decimal EscrowDelta { get; set; }
    public decimal WalletBalanceAfter { get; set; }
    public decimal EscrowBalanceAfter { get; set; }
    public string Currency { get; set; } = "KRW";
    public string? Counterparty { get; set; }
    public string? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public string? Memo { get; set; }
    public DateTime CreatedAt { get; set; }
}
