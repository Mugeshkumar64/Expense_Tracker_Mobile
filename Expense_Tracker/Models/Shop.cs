using System.Text.Json.Serialization;

namespace Expense_Tracker.Models;

public class Shop
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("contact")]
    public string? Contact { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("isDemo")]
    public bool IsDemo { get; set; }

    [JsonPropertyName("totalSpending")]
    public decimal TotalSpending { get; set; }

    [JsonPropertyName("productsCount")]
    public int ProductsCount { get; set; }

    [JsonPropertyName("lowestPriceMatchesCount")]
    public int LowestPriceMatchesCount { get; set; }

    public string FormattedTotalSpending => $"₹{TotalSpending:N0}";
    public string ProductsText => $"{ProductsCount} products";
    public string LowestMatchesText => $"{LowestPriceMatchesCount} lowest-price matches";
    public string LocationDisplay => string.IsNullOrWhiteSpace(Location) ? "No location set" : Location;
}

public class CreateShopRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("contact")]
    public string? Contact { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
