using System.Text.Json.Serialization;

namespace Expense_Tracker.Models;

public class Category
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = "expense"; // "expense" or "income"

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("totalEntries")]
    public int TotalEntries { get; set; }

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    public bool IsExpense => Type.Equals("expense", StringComparison.OrdinalIgnoreCase);
    public string BadgeColor => IsExpense ? "#E11D48" : "#059669";
    public string BadgeBgColor => IsExpense ? "#FFF1F2" : "#ECFDF5";
    public string FormattedTotal => $"₹{TotalAmount:N0}";
    public string EntriesSummary => $"{TotalEntries} transactions";

    public string Icon => Name.ToLower() switch
    {
        var n when n.Contains("food") || n.Contains("dining") || n.Contains("restaurant") || n.Contains("eat") => "🍔",
        var n when n.Contains("grocer") || n.Contains("supermarket") => "🛒",
        var n when n.Contains("coffee") || n.Contains("tea") => "☕",
        var n when n.Contains("transport") || n.Contains("fuel") || n.Contains("bus") || n.Contains("cab") => "🚌",
        var n when n.Contains("travel") || n.Contains("flight") || n.Contains("trip") => "✈️",
        var n when n.Contains("shop") || n.Contains("cloth") || n.Contains("mall") => "🛍️",
        var n when n.Contains("bill") || n.Contains("electric") || n.Contains("recharge") => "💡",
        var n when n.Contains("rent") || n.Contains("home") || n.Contains("house") => "🏠",
        var n when n.Contains("health") || n.Contains("medic") || n.Contains("pharma") => "💊",
        var n when n.Contains("entertain") || n.Contains("movie") || n.Contains("game") => "🎬",
        var n when n.Contains("salary") || n.Contains("wage") || n.Contains("income") => "💼",
        var n when n.Contains("freelanc") || n.Contains("project") => "💻",
        var n when n.Contains("business") || n.Contains("sales") => "🏪",
        var n when n.Contains("invest") || n.Contains("dividend") => "📈",
        _ => "🏷️"
    };

    public string IconTint => (Name.Length % 4) switch
    {
        0 => "#EFF6FF",
        1 => "#FFF7ED",
        2 => "#ECFDF5",
        _ => "#F5F3FF"
    };
}

public class CreateCategoryRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = "expense";
}
