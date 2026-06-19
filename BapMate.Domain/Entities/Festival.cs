namespace BapMate.Domain.Entities;

public class Festival
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
    public string ImagesJson { get; set; } = "[]";
    public string TagsJson { get; set; } = "[]";
    public int ParticipantCount { get; set; }
    public int? MaxParticipants { get; set; }
    public int Likes { get; set; }
    public bool IsLiked { get; set; }
    public decimal? TicketPrice { get; set; }
    public bool IsBookingRequired { get; set; }
    public string? BookingUrl { get; set; }
    public string FacilitiesJson { get; set; } = "[]";
    public string AccessibilityJson { get; set; } = "[]";
    public bool Parking { get; set; }
    public string PublicTransportJson { get; set; } = "[]";
    public string? Website { get; set; }
    public string? SocialMediaJson { get; set; }
    public string ContentJson { get; set; } = "{}";
    public string? WeatherJson { get; set; }
    public string? NearbyRestaurantsJson { get; set; }
    public string ReviewsJson { get; set; } = "[]";
}
