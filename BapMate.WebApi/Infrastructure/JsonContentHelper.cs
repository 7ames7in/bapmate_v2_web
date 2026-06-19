using System.Text.Json;

namespace BapMate.WebApi.Infrastructure;

public static class JsonContentHelper
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public static IReadOnlyList<T> DeserializeList<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Array.Empty<T>();
        }

        return JsonSerializer.Deserialize<IReadOnlyList<T>>(json, Options) ?? Array.Empty<T>();
    }

    public static T? DeserializeObject<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json, Options);
    }

    public static IReadOnlyDictionary<TKey, TValue> DeserializeDictionary<TKey, TValue>(string? json)
        where TKey : notnull
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new Dictionary<TKey, TValue>();
        }

        return JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(json, Options)
               ?? new Dictionary<TKey, TValue>();
    }

    public static JsonElement? DeserializeElement(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    public static string Serialize<T>(T value) =>
        JsonSerializer.Serialize(value, Options);

    public static string? SerializeElement(JsonElement? element) =>
        element.HasValue ? element.Value.GetRawText() : null;
}
