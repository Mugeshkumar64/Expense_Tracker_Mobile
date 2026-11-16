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
    public string BadgeColor => IsExpense ? "#F43F5E" : "#10B981";
    public string FormattedTotal => $"₹{TotalAmount:N0}";
    public string EntriesSummary => $"{TotalEntries} transactions";
}

public class CreateCategoryRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = "expense";
}
