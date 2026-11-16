using Expense_Tracker.Models;

namespace Expense_Tracker.Services;

public interface IApiService
{
    string BaseUrl { get; set; }

    // Auth
    Task<ApiResponse<UserDto>?> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<UserDto>?> LoginAsync(LoginRequest request);
    Task<ApiResponse<UserDto>?> GetCurrentUserAsync(string email);

    // Dashboard
    Task<ApiResponse<DashboardSummary>?> GetDashboardSummaryAsync();

    // Transactions
    Task<ApiResponse<List<Transaction>>?> GetTransactionsAsync(string? type = null, string? category = null, string? shop = null, string? dateRange = null, string? search = null, string? sort = null);
    Task<ApiResponse<Transaction>?> GetTransactionByIdAsync(int id);
    Task<ApiResponse<Transaction>?> CreateTransactionAsync(CreateTransactionRequest request);
    Task<ApiResponse<Transaction>?> UpdateTransactionAsync(int id, CreateTransactionRequest request);
    Task<ApiResponse?> DeleteTransactionAsync(int id);

    // Categories
    Task<ApiResponse<List<Category>>?> GetCategoriesAsync(string? type = null, string? search = null);
    Task<ApiResponse<Category>?> CreateCategoryAsync(CreateCategoryRequest request);
    Task<ApiResponse?> DeleteCategoryAsync(int id);
    Task<ApiResponse?> DeleteCategoryByNameAsync(string name);

    // Purchases (Smart Shopping)
    Task<ApiResponse<List<Purchase>>?> GetPurchasesAsync(string? search = null);
    Task<ApiResponse<Purchase>?> CreatePurchaseAsync(CreatePurchaseRequest request, int? listItemId = null);
    Task<ApiResponse?> DeletePurchaseAsync(int id);

    // Shopping List
    Task<ApiResponse<ShoppingListSummary>?> GetShoppingListSummaryAsync();
    Task<ApiResponse<ShoppingListItem>?> AddShoppingListItemAsync(CreateShoppingListItemRequest request);
    Task<ApiResponse?> DeleteShoppingListItemAsync(int id);
    Task<ApiResponse<Purchase>?> BuyShoppingListItemAsync(int id, CreatePurchaseRequest purchaseRequest);

    // Price Comparison
    Task<ApiResponse<PriceComparisonResult>?> ComparePricesAsync(string product, decimal quantity = 1);

    // Savings
    Task<ApiResponse<SavingsSummary>?> GetSavingsSummaryAsync();

    // Shops
    Task<ApiResponse<List<Shop>>?> GetShopsAsync();
    Task<ApiResponse<Shop>?> CreateShopAsync(CreateShopRequest request);
    Task<ApiResponse?> DeleteShopAsync(int id);

    // Products
    Task<ApiResponse<List<string>>?> GetProductNamesAsync();

    // Settings
    Task<ApiResponse<UserSettings>?> GetSettingsAsync();
    Task<ApiResponse<UserSettings>?> UpdateSettingsAsync(UpdateSettingsRequest request);
    Task<ApiResponse?> LoadDemoDataAsync();
    Task<ApiResponse?> ClearDemoDataAsync();
    Task<ApiResponse?> ResetDatabaseAsync();
}
