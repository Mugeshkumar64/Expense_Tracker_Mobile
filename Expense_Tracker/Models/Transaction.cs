using System.Text.Json.Serialization;

namespace Expense_Tracker.Models;

public class Transaction
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "expense"; // "expense" or "income"

    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }

    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("paymentMethod")]
    public string PaymentMethod { get; set; } = "UPI";

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("shop")]
    public string? Shop { get; set; }

    [JsonPropertyName("purchaseId")]
    public int? PurchaseId { get; set; }

    [JsonPropertyName("isDemo")]
    public bool IsDemo { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    public bool IsIncome => Type.Equals("income", StringComparison.OrdinalIgnoreCase);
    public string AmountColor => IsIncome ? "#059669" : "#E11D48";
    public string FormattedAmount => (IsIncome ? "+₹" : "−₹") + $"{Amount:N0}";
    public string FormattedDate => Date.ToString("yyyy-MM-dd");
    public string TypeIcon => IsIncome ? "↓" : "↑";
    public string Subtitle => $"{CategoryName} · {FormattedDate}{(string.IsNullOrWhiteSpace(Shop) ? "" : " · " + Shop)}";
}

public class CreateTransactionRequest
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "expense";

    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("categoryName")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("paymentMethod")]
    public string PaymentMethod { get; set; } = "UPI";

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("shop")]
    public string? Shop { get; set; }
}
