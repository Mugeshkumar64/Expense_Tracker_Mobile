using System.Net.Http.Json;
using System.Text.Json;
using Expense_Tracker.Models;

namespace Expense_Tracker.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public static string DefaultBaseUrl => AppConfigService.GetDefaultBaseUrl();

    public string BaseUrl
    {
        get => Preferences.Default.Get("ApiBaseUrl", DefaultBaseUrl);
        set
        {
            var url = value.Trim();
            if (!url.EndsWith('/')) url += "/";
            Preferences.Default.Set("ApiBaseUrl", url);
            _httpClient.BaseAddress = new Uri(url);
        }
    }

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(20)
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    // Auth
    public async Task<ApiResponse<UserDto>?> RegisterAsync(RegisterRequest request) =>
        await PostAsync<RegisterRequest, UserDto>("api/auth/register", request);

    public async Task<ApiResponse<UserDto>?> LoginAsync(LoginRequest request) =>
        await PostAsync<LoginRequest, UserDto>("api/auth/login", request);

    public async Task<ApiResponse<UserDto>?> GetCurrentUserAsync(string email) =>
        await GetAsync<UserDto>($"api/auth/me?email={Uri.EscapeDataString(email)}");

    // Dashboard
    public async Task<ApiResponse<DashboardSummary>?> GetDashboardSummaryAsync() =>
        await GetAsync<DashboardSummary>("api/dashboard/summary");

    // Transactions
    public async Task<ApiResponse<List<Transaction>>?> GetTransactionsAsync(
        string? type = null, string? category = null, string? shop = null,
        string? dateRange = null, string? search = null, string? sort = null)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(type)) query.Add($"type={Uri.EscapeDataString(type)}");
        if (!string.IsNullOrWhiteSpace(category)) query.Add($"category={Uri.EscapeDataString(category)}");
        if (!string.IsNullOrWhiteSpace(shop)) query.Add($"shop={Uri.EscapeDataString(shop)}");
        if (!string.IsNullOrWhiteSpace(dateRange)) query.Add($"dateRange={Uri.EscapeDataString(dateRange)}");
        if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={Uri.EscapeDataString(search)}");
        if (!string.IsNullOrWhiteSpace(sort)) query.Add($"sort={Uri.EscapeDataString(sort)}");

        var path = "api/transactions" + (query.Any() ? "?" + string.Join("&", query) : "");
        return await GetAsync<List<Transaction>>(path);
    }

    public async Task<ApiResponse<Transaction>?> GetTransactionByIdAsync(int id) =>
        await GetAsync<Transaction>($"api/transactions/{id}");

    public async Task<ApiResponse<Transaction>?> CreateTransactionAsync(CreateTransactionRequest request) =>
        await PostAsync<CreateTransactionRequest, Transaction>("api/transactions", request);

    public async Task<ApiResponse<Transaction>?> UpdateTransactionAsync(int id, CreateTransactionRequest request) =>
        await PutAsync<CreateTransactionRequest, Transaction>($"api/transactions/{id}", request);

    public async Task<ApiResponse?> DeleteTransactionAsync(int id) =>
        await DeleteAsync($"api/transactions/{id}");

    // Categories
    public async Task<ApiResponse<List<Category>>?> GetCategoriesAsync(string? type = null, string? search = null)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(type)) query.Add($"type={Uri.EscapeDataString(type)}");
        if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={Uri.EscapeDataString(search)}");
        var path = "api/categories" + (query.Any() ? "?" + string.Join("&", query) : "");
        return await GetAsync<List<Category>>(path);
    }

    public async Task<ApiResponse<Category>?> CreateCategoryAsync(CreateCategoryRequest request) =>
        await PostAsync<CreateCategoryRequest, Category>("api/categories", request);

    public async Task<ApiResponse?> DeleteCategoryAsync(int id) =>
        await DeleteAsync($"api/categories/{id}");

    public async Task<ApiResponse?> DeleteCategoryByNameAsync(string name) =>
        await DeleteAsync($"api/categories/by-name/{Uri.EscapeDataString(name)}");

    // Purchases (Smart Shopping)
    public async Task<ApiResponse<List<Purchase>>?> GetPurchasesAsync(string? search = null)
    {
        var path = "api/purchases" + (!string.IsNullOrWhiteSpace(search) ? $"?search={Uri.EscapeDataString(search)}" : "");
        return await GetAsync<List<Purchase>>(path);
    }

    public async Task<ApiResponse<Purchase>?> CreatePurchaseAsync(CreatePurchaseRequest request, int? listItemId = null)
    {
        var path = "api/purchases" + (listItemId.HasValue ? $"?listItemId={listItemId.Value}" : "");
        return await PostAsync<CreatePurchaseRequest, Purchase>(path, request);
    }

    public async Task<ApiResponse?> DeletePurchaseAsync(int id) =>
        await DeleteAsync($"api/purchases/{id}");

    // Shopping List
    public async Task<ApiResponse<ShoppingListSummary>?> GetShoppingListSummaryAsync() =>
        await GetAsync<ShoppingListSummary>("api/shoppinglist");

    public async Task<ApiResponse<ShoppingListItem>?> AddShoppingListItemAsync(CreateShoppingListItemRequest request) =>
        await PostAsync<CreateShoppingListItemRequest, ShoppingListItem>("api/shoppinglist", request);

    public async Task<ApiResponse?> DeleteShoppingListItemAsync(int id) =>
        await DeleteAsync($"api/shoppinglist/{id}");

    public async Task<ApiResponse<Purchase>?> BuyShoppingListItemAsync(int id, CreatePurchaseRequest purchaseRequest) =>
        await PostAsync<CreatePurchaseRequest, Purchase>($"api/shoppinglist/{id}/buy", purchaseRequest);

    // Price Comparison
    public async Task<ApiResponse<PriceComparisonResult>?> ComparePricesAsync(string product, decimal quantity = 1) =>
        await GetAsync<PriceComparisonResult>($"api/price-comparison?product={Uri.EscapeDataString(product)}&quantity={quantity}");

    // Savings
    public async Task<ApiResponse<SavingsSummary>?> GetSavingsSummaryAsync() =>
        await GetAsync<SavingsSummary>("api/savings/summary");

    // Shops
    public async Task<ApiResponse<List<Shop>>?> GetShopsAsync() =>
        await GetAsync<List<Shop>>("api/shops");

    public async Task<ApiResponse<Shop>?> CreateShopAsync(CreateShopRequest request) =>
        await PostAsync<CreateShopRequest, Shop>("api/shops", request);

    public async Task<ApiResponse?> DeleteShopAsync(int id) =>
        await DeleteAsync($"api/shops/{id}");

    // Products
    public async Task<ApiResponse<List<string>>?> GetProductNamesAsync() =>
        await GetAsync<List<string>>("api/products/names");

    // Settings
    public async Task<ApiResponse<UserSettings>?> GetSettingsAsync() =>
        await GetAsync<UserSettings>("api/settings");

    public async Task<ApiResponse<UserSettings>?> UpdateSettingsAsync(UpdateSettingsRequest request) =>
        await PutAsync<UpdateSettingsRequest, UserSettings>("api/settings", request);

    public async Task<ApiResponse?> LoadDemoDataAsync() =>
        await PostEmptyAsync("api/settings/demo/load");

    public async Task<ApiResponse?> ClearDemoDataAsync() =>
        await PostEmptyAsync("api/settings/demo/clear");

    public async Task<ApiResponse?> ResetDatabaseAsync() =>
        await PostEmptyAsync("api/settings/reset");

    // Internal HTTP Helpers
    private async Task<ApiResponse<T>?> GetAsync<T>(string uri)
    {
        try
        {
            var response = await _httpClient.GetAsync(uri);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Message = $"Network error: {ex.Message}" };
        }
    }

    private async Task<ApiResponse<TResponse>?> PostAsync<TRequest, TResponse>(string uri, TRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(uri, request, _jsonOptions);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<TResponse>>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse<TResponse> { Success = false, Message = $"Network error: {ex.Message}" };
        }
    }

    private async Task<ApiResponse?> PostEmptyAsync(string uri)
    {
        try
        {
            var response = await _httpClient.PostAsync(uri, null);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = $"Network error: {ex.Message}" };
        }
    }

    private async Task<ApiResponse<TResponse>?> PutAsync<TRequest, TResponse>(string uri, TRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(uri, request, _jsonOptions);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<TResponse>>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse<TResponse> { Success = false, Message = $"Network error: {ex.Message}" };
        }
    }

    private async Task<ApiResponse?> DeleteAsync(string uri)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(uri);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = $"Network error: {ex.Message}" };
        }
    }
}
