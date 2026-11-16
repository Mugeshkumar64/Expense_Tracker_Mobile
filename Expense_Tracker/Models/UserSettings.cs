using System.Text.Json.Serialization;

namespace Expense_Tracker.Models;

public class UserSettings
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "₹";

    [JsonPropertyName("monthBudget")]
    public decimal MonthBudget { get; set; }

    [JsonPropertyName("shopBudget")]
    public decimal ShopBudget { get; set; }

    [JsonPropertyName("listBudget")]
    public decimal ListBudget { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

public class UpdateSettingsRequest
{
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "₹";

    [JsonPropertyName("monthBudget")]
    public decimal MonthBudget { get; set; }

    [JsonPropertyName("shopBudget")]
    public decimal ShopBudget { get; set; }

    [JsonPropertyName("listBudget")]
    public decimal ListBudget { get; set; }
}
