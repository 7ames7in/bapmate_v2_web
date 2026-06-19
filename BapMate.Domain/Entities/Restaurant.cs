namespace BapMate.Domain.Entities;

public class Restaurant
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Distance { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string MenuJson { get; set; } = "[]";
    public string ReviewsJson { get; set; } = "[]";
    public string CouponsJson { get; set; } = "[]";
    public string? EventHistoryJson { get; set; }
    public int? TotalReviews { get; set; }
    public string? PriceRange { get; set; }
    public string? BusinessHours { get; set; }
    public string? Phone { get; set; }
}
