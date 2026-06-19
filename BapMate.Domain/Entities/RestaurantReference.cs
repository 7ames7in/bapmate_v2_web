namespace BapMate.Domain.Entities;

public class RestaurantReference
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string TagsJson { get; set; } = "[]";
}
