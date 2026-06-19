namespace BapMate.Domain.Entities;

public class SupportRequest
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Story { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool Verified { get; set; }
    public double Progress { get; set; }
    public string ExpiresAt { get; set; } = string.Empty;
}
