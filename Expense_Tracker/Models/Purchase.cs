using System.Text.Json.Serialization;

namespace Expense_Tracker.Models;

public class Purchase
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }

    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = "kg";

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }

    [JsonPropertyName("shopId")]
    public int? ShopId { get; set; }

    [JsonPropertyName("shopName")]
    public string ShopName { get; set; } = string.Empty;

    [JsonPropertyName("shopLocation")]
    public string? ShopLocation { get; set; }

    [JsonPropertyName("paymentMethod")]
    public string PaymentMethod { get; set; } = "UPI";

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("fromList")]
    public bool FromList { get; set; }

    [JsonPropertyName("isDemo")]
    public bool IsDemo { get; set; }

    [JsonPropertyName("transactionId")]
    public int? TransactionId { get; set; }

    public string FormattedTotal => $"₹{TotalPrice:N2}";
    public string FormattedUnitPrice => $"₹{UnitPrice:N2}/{Unit}";
    public string FormattedQty => $"{Quantity:0.###} {Unit}";
    public string FormattedDate => Date.ToString("yyyy-MM-dd");
    public string Subtitle => $"{FormattedQty} · {ShopName} · {FormattedDate}";
}

public class CreatePurchaseRequest
{
    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("categoryName")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; } = 1;

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = "kg";

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("shopName")]
    public string ShopName { get; set; } = string.Empty;

    [JsonPropertyName("shopLocation")]
    public string? ShopLocation { get; set; }

    [JsonPropertyName("paymentMethod")]
    public string PaymentMethod { get; set; } = "UPI";

    [JsonPropertyName("date")]
    public DateTime Date { get; set; } = DateTime.Now;

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("fromList")]
    public bool FromList { get; set; }
}
