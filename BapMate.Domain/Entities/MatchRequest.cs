namespace BapMate.Domain.Entities;

public class MatchRequest
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double RadiusKm { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public string PreferredAt { get; set; } = string.Empty;
    public int PartySize { get; set; }
    public string GenderPreference { get; set; } = "any";
    public string? AgeRange { get; set; }
    public string InterestsJson { get; set; } = "[]";
    public bool DepositRequired { get; set; }
    public string Status { get; set; } = "open";
    public string CreatedAt { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public string? ConfirmedWith { get; set; }
    public string? Notes { get; set; }
    public string? RestaurantId { get; set; }
    public bool AllowRestaurantSuggestions { get; set; }
    public string SuggestionsJson { get; set; } = "[]";
}
