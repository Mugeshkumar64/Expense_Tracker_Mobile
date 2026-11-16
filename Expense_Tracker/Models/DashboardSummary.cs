using System.Text.Json.Serialization;

namespace Expense_Tracker.Models;

public class DashboardSummary
{
    [JsonPropertyName("totalNetBalance")]
    public decimal TotalNetBalance { get; set; }

    [JsonPropertyName("incomeThisMonth")]
    public decimal IncomeThisMonth { get; set; }

    [JsonPropertyName("expenseThisMonth")]
    public decimal ExpenseThisMonth { get; set; }

    [JsonPropertyName("netSavingsThisMonth")]
    public decimal NetSavingsThisMonth { get; set; }

    [JsonPropertyName("savingsPercentage")]
    public int SavingsPercentage { get; set; }

    [JsonPropertyName("shoppingBudgetLeft")]
    public decimal ShoppingBudgetLeft { get; set; }

    [JsonPropertyName("shoppingSpentThisMonth")]
    public decimal ShoppingSpentThisMonth { get; set; }

    [JsonPropertyName("shoppingBudget")]
    public decimal ShoppingBudget { get; set; }

    [JsonPropertyName("hasShoppingBudget")]
    public bool HasShoppingBudget { get; set; }

    [JsonPropertyName("monthlyBudget")]
    public decimal MonthlyBudget { get; set; }

    [JsonPropertyName("monthlyBudgetUsedPercentage")]
    public int MonthlyBudgetUsedPercentage { get; set; }

    [JsonPropertyName("hasMonthlyBudget")]
    public bool HasMonthlyBudget { get; set; }

    [JsonPropertyName("potentialSavings")]
    public decimal PotentialSavings { get; set; }

    [JsonPropertyName("actualSavings")]
    public decimal ActualSavings { get; set; }

    [JsonPropertyName("monthlyCashFlow")]
    public List<MonthlyCashFlowItem> MonthlyCashFlow { get; set; } = new();

    [JsonPropertyName("expenseAllocation")]
    public List<CategoryExpenseAllocationItem> ExpenseAllocation { get; set; } = new();

    [JsonPropertyName("recentTransactions")]
    public List<Transaction> RecentTransactions { get; set; } = new();

    [JsonPropertyName("smartInsights")]
    public List<string> SmartInsights { get; set; } = new();

    [JsonPropertyName("priceMovers")]
    public List<PriceMoverItem> PriceMovers { get; set; } = new();

    [JsonPropertyName("cheapestShops")]
    public List<CheapestShopItem> CheapestShops { get; set; } = new();

    public string FormattedBalance => $"₹{TotalNetBalance:N0}";
    public string FormattedIncome => $"+₹{IncomeThisMonth:N0}";
    public string FormattedExpense => $"-₹{ExpenseThisMonth:N0}";
    public string FormattedSavings => $"₹{NetSavingsThisMonth:N0}";
    public string FormattedSavingsPct => $"{SavingsPercentage}% saved";
    public string FormattedShoppingBudgetLeft => HasShoppingBudget ? $"₹{ShoppingBudgetLeft:N0}" : "Not set";
    public string FormattedBudgetUsed => HasMonthlyBudget ? $"{MonthlyBudgetUsedPercentage}% used" : "Not set";
    public string FormattedPotentialSavings => $"₹{PotentialSavings:N0}";
    public string FormattedActualSavings => $"₹{ActualSavings:N0}";
}

public class MonthlyCashFlowItem
{
    [JsonPropertyName("monthKey")]
    public string MonthKey { get; set; } = string.Empty;

    [JsonPropertyName("monthLabel")]
    public string MonthLabel { get; set; } = string.Empty;

    [JsonPropertyName("income")]
    public decimal Income { get; set; }

    [JsonPropertyName("expense")]
    public decimal Expense { get; set; }
}

public class CategoryExpenseAllocationItem
{
    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("percentage")]
    public decimal Percentage { get; set; }

    public string FormattedAmount => $"₹{Amount:N0}";
    public double ProgressFraction => (double)(Percentage / 100m);
}

public class PriceMoverItem
{
    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("shopName")]
    public string ShopName { get; set; } = string.Empty;

    [JsonPropertyName("baseUnit")]
    public string BaseUnit { get; set; } = "kg";

    [JsonPropertyName("previousPrice")]
    public decimal PreviousPrice { get; set; }

    [JsonPropertyName("latestPrice")]
    public decimal LatestPrice { get; set; }

    [JsonPropertyName("percentageChange")]
    public decimal PercentageChange { get; set; }

    [JsonPropertyName("isIncrease")]
    public bool IsIncrease { get; set; }

    public string ChangeIcon => IsIncrease ? "▲" : "▼";
    public string ChangeColor => IsIncrease ? "#F43F5E" : "#10B981";
    public string FormattedChange => $"{ChangeIcon} {PercentageChange:0.0}%";
    public string PriceSummary => $"{ProductName}: ₹{PreviousPrice:N2} → ₹{LatestPrice:N2}/{BaseUnit} at {ShopName}";
}

public class CheapestShopItem
{
    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("shopName")]
    public string ShopName { get; set; } = string.Empty;

    [JsonPropertyName("baseUnit")]
    public string BaseUnit { get; set; } = "kg";

    [JsonPropertyName("lowestPrice")]
    public decimal LowestPrice { get; set; }

    public string DisplayText => $"{ProductName} → {ShopName} (₹{LowestPrice:N2}/{BaseUnit})";
}
