using System.Text.Json.Serialization;

namespace Expense_Tracker.Models;

public class PriceComparisonResult
{
    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("baseUnit")]
    public string BaseUnit { get; set; } = "kg";

    [JsonPropertyName("plannedQuantity")]
    public decimal PlannedQuantity { get; set; }

    [JsonPropertyName("lowestRecordedPrice")]
    public decimal LowestRecordedPrice { get; set; }

    [JsonPropertyName("lowestRecordedDate")]
    public DateTime LowestRecordedDate { get; set; }

    [JsonPropertyName("cheapestShop")]
    public string CheapestShop { get; set; } = string.Empty;

    [JsonPropertyName("lastPurchasePrice")]
    public decimal LastPurchasePrice { get; set; }

    [JsonPropertyName("lastPurchaseShop")]
    public string LastPurchaseShop { get; set; } = string.Empty;

    [JsonPropertyName("lastPurchaseDate")]
    public DateTime LastPurchaseDate { get; set; }

    [JsonPropertyName("savingVsLastPurchasePerUnit")]
    public decimal SavingVsLastPurchasePerUnit { get; set; }

    [JsonPropertyName("totalSavingVsLastPurchase")]
    public decimal TotalSavingVsLastPurchase { get; set; }

    [JsonPropertyName("suggestionText")]
    public string SuggestionText { get; set; } = string.Empty;

    [JsonPropertyName("isOutdated")]
    public bool IsOutdated { get; set; }

    [JsonPropertyName("latestRecordedDate")]
    public DateTime LatestRecordedDate { get; set; }

    [JsonPropertyName("shopRows")]
    public List<ShopComparisonRow> ShopRows { get; set; } = new();

    [JsonPropertyName("priceHistory")]
    public List<ProductPriceHistoryRecord> PriceHistory { get; set; } = new();

    public string FormattedLowestPrice => $"₹{LowestRecordedPrice:N2}/{BaseUnit}";
    public string FormattedLastPaid => $"₹{LastPurchasePrice:N2}/{BaseUnit}";
    public string FormattedSaving => $"₹{TotalSavingVsLastPurchase:N2}";
}

public class ShopComparisonRow
{
    [JsonPropertyName("shopName")]
    public string ShopName { get; set; } = string.Empty;

    [JsonPropertyName("pricePerBaseUnit")]
    public decimal PricePerBaseUnit { get; set; }

    [JsonPropertyName("estimatedTotal")]
    public decimal EstimatedTotal { get; set; }

    [JsonPropertyName("lastRecordedDate")]
    public DateTime LastRecordedDate { get; set; }

    [JsonPropertyName("isLowestPrice")]
    public bool IsLowestPrice { get; set; }

    [JsonPropertyName("priceDifferencePerUnit")]
    public decimal PriceDifferencePerUnit { get; set; }

    [JsonPropertyName("isPotentiallyOutdated")]
    public bool IsPotentiallyOutdated { get; set; }

    public string FormattedPrice => $"₹{PricePerBaseUnit:N2}";
    public string FormattedTotal => $"₹{EstimatedTotal:N2}";
    public string FormattedDate => LastRecordedDate.ToString("yyyy-MM-dd");
    public string StatusBadge => IsLowestPrice ? "Lowest Price" : $"+₹{PriceDifferencePerUnit:N2}";
    public string StatusColor => IsLowestPrice ? "#059669" : "#E11D48";
}

public class ProductPriceHistoryRecord
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("shopName")]
    public string ShopName { get; set; } = string.Empty;

    [JsonPropertyName("normalizedPrice")]
    public decimal NormalizedPrice { get; set; }

    [JsonPropertyName("baseUnit")]
    public string BaseUnit { get; set; } = "kg";

    [JsonPropertyName("percentageOfMax")]
    public decimal PercentageOfMax { get; set; }

    public string FormattedDate => Date.ToString("yyyy-MM-dd");
    public string FormattedPrice => $"₹{NormalizedPrice:N2}/{BaseUnit}";
}
