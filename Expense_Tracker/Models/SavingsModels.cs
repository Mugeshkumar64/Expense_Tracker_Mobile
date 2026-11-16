using System.Text.Json.Serialization;

namespace Expense_Tracker.Models;

public class SavingsSummary
{
    [JsonPropertyName("actualSavings")]
    public decimal ActualSavings { get; set; }

    [JsonPropertyName("potentialSavings")]
    public decimal PotentialSavings { get; set; }

    [JsonPropertyName("shoppingListPlanningSavings")]
    public decimal ShoppingListPlanningSavings { get; set; }

    [JsonPropertyName("actualBoughtViaListSavings")]
    public decimal ActualBoughtViaListSavings { get; set; }

    [JsonPropertyName("actualSavingsLog")]
    public List<ActualSavingsLogItem> ActualSavingsLog { get; set; } = new();

    [JsonPropertyName("monthlyActualSavings")]
    public List<MonthlySavingsTrendItem> MonthlyActualSavings { get; set; } = new();

    [JsonPropertyName("highestPotentialSavings")]
    public List<PotentialSavingsItem> HighestPotentialSavings { get; set; } = new();

    public string FormattedActualSavings => $"₹{ActualSavings:N0}";
    public string FormattedPotentialSavings => $"₹{PotentialSavings:N0}";
    public string FormattedListSavings => $"₹{ShoppingListPlanningSavings:N0}";
    public string FormattedListActualSavings => $"₹{ActualBoughtViaListSavings:N0}";
}

public class ActualSavingsLogItem
{
    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("shopName")]
    public string ShopName { get; set; } = string.Empty;

    [JsonPropertyName("savedAmount")]
    public decimal SavedAmount { get; set; }

    [JsonPropertyName("paidPricePerUnit")]
    public decimal PaidPricePerUnit { get; set; }

    [JsonPropertyName("usualPricePerUnit")]
    public decimal UsualPricePerUnit { get; set; }

    [JsonPropertyName("usualShopName")]
    public string UsualShopName { get; set; } = string.Empty;

    [JsonPropertyName("usualPurchaseDate")]
    public DateTime UsualPurchaseDate { get; set; }

    [JsonPropertyName("purchaseDate")]
    public DateTime PurchaseDate { get; set; }

    [JsonPropertyName("baseUnit")]
    public string BaseUnit { get; set; } = "kg";

    public string FormattedSaved => $"Saved ₹{SavedAmount:N2}";
    public string SummaryText => $"{ProductName} at {ShopName}";
    public string DetailsText => $"Paid ₹{PaidPricePerUnit:N2}/{BaseUnit} vs ₹{UsualPricePerUnit:N2}/{BaseUnit} at usual shop {UsualShopName} · {PurchaseDate:yyyy-MM-dd}";
}

public class MonthlySavingsTrendItem
{
    [JsonPropertyName("monthKey")]
    public string MonthKey { get; set; } = string.Empty;

    [JsonPropertyName("monthName")]
    public string MonthName { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("percentage")]
    public decimal Percentage { get; set; }

    public string FormattedAmount => $"₹{Amount:N0}";
    public double ProgressFraction => (double)(Percentage / 100m);
}

public class PotentialSavingsItem
{
    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("lastPaidPrice")]
    public decimal LastPaidPrice { get; set; }

    [JsonPropertyName("lastShop")]
    public string LastShop { get; set; } = string.Empty;

    [JsonPropertyName("lowestPrice")]
    public decimal LowestPrice { get; set; }

    [JsonPropertyName("lowestShop")]
    public string LowestShop { get; set; } = string.Empty;

    [JsonPropertyName("estimatedSaving")]
    public decimal EstimatedSaving { get; set; }

    [JsonPropertyName("lastQuantity")]
    public decimal LastQuantity { get; set; }

    [JsonPropertyName("baseUnit")]
    public string BaseUnit { get; set; } = "kg";

    public string FormattedSaving => $"Save ₹{EstimatedSaving:N2}";
    public string DetailsText => $"Last paid ₹{LastPaidPrice:N2} at {LastShop}; lowest ₹{LowestPrice:N2} at {LowestShop} (on {LastQuantity:0.###} {BaseUnit})";
}
