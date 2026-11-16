using System.Text.Json.Serialization;

namespace Expense_Tracker.Models;

public class ShoppingListItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; } = 1;

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = "kg";

    [JsonPropertyName("isPurchased")]
    public bool IsPurchased { get; set; }

    [JsonPropertyName("hasRecordedPrice")]
    public bool HasRecordedPrice { get; set; }

    [JsonPropertyName("cheapestShop")]
    public string? CheapestShop { get; set; }

    [JsonPropertyName("cheapestPricePerBaseUnit")]
    public decimal? CheapestPricePerBaseUnit { get; set; }

    [JsonPropertyName("baseUnit")]
    public string? BaseUnit { get; set; }

    [JsonPropertyName("estimatedCost")]
    public decimal? EstimatedCost { get; set; }

    [JsonPropertyName("estimatedSavingsVsUsual")]
    public decimal? EstimatedSavingsVsUsual { get; set; }

    public string FormattedQty => $"{Quantity:0.###} {Unit}";
    public string FormattedCost => EstimatedCost.HasValue ? $"₹{EstimatedCost.Value:N2}" : "Price N/A";
    public string PriceNote => HasRecordedPrice
        ? $"Cheapest: {CheapestShop} · ₹{CheapestPricePerBaseUnit:N2}/{BaseUnit}"
        : "No recorded price available";
}

public class CreateShoppingListItemRequest
{
    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; } = 1;

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = "kg";
}

public class ShoppingListSummary
{
    [JsonPropertyName("items")]
    public List<ShoppingListItem> Items { get; set; } = new();

    [JsonPropertyName("totalEstimatedCost")]
    public decimal TotalEstimatedCost { get; set; }

    [JsonPropertyName("estimatedSavingsVsUsual")]
    public decimal EstimatedSavingsVsUsual { get; set; }

    [JsonPropertyName("totalItemsCount")]
    public int TotalItemsCount { get; set; }

    [JsonPropertyName("pricedItemsCount")]
    public int PricedItemsCount { get; set; }

    [JsonPropertyName("unpricedItemsCount")]
    public int UnpricedItemsCount { get; set; }

    [JsonPropertyName("targetBudget")]
    public decimal TargetBudget { get; set; }

    [JsonPropertyName("budgetDifference")]
    public decimal BudgetDifference { get; set; }

    [JsonPropertyName("isWithinBudget")]
    public bool IsWithinBudget { get; set; }

    public string FormattedEstimatedCost => $"₹{TotalEstimatedCost:N2}";
    public string FormattedEstimatedSavings => $"₹{EstimatedSavingsVsUsual:N2}";
    public string BudgetStatusText => TargetBudget > 0
        ? (IsWithinBudget ? $"Within budget — ₹{BudgetDifference:N0} left of ₹{TargetBudget:N0}" : $"Over budget by ₹{-BudgetDifference:N0}")
        : "Set a target budget in Settings";
}
