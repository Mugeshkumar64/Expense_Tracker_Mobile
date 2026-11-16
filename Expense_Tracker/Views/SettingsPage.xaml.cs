using Expense_Tracker.Models;
using Expense_Tracker.Services;

namespace Expense_Tracker.Views;

public partial class SettingsPage : ContentPage
{
    private readonly IApiService _apiService;
    private readonly IAuthManager _authManager;

    public SettingsPage() : this(new ApiService(), new AuthManager())
    {
    }

    public SettingsPage(IApiService apiService, IAuthManager authManager)
    {
        InitializeComponent();
        _apiService = apiService;
        _authManager = authManager;
        TxtApiUrl.Text = _apiService.BaseUrl;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        TxtApiUrl.Text = _apiService.BaseUrl;
        await LoadSettingsAsync();
    }

    private async Task LoadSettingsAsync()
    {
        try
        {
            var res = await _apiService.GetSettingsAsync();
            if (res != null && res.Success && res.Data != null)
            {
                var s = res.Data;
                int curIdx = PickerCurrency.Items.IndexOf(s.Currency);
                PickerCurrency.SelectedIndex = curIdx >= 0 ? curIdx : 0;

                TxtMonthBudget.Text = s.MonthBudget > 0 ? s.MonthBudget.ToString("0.##") : string.Empty;
                TxtShopBudget.Text = s.ShopBudget > 0 ? s.ShopBudget.ToString("0.##") : string.Empty;
                TxtListBudget.Text = s.ListBudget > 0 ? s.ListBudget.ToString("0.##") : string.Empty;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not load settings: {ex.Message}", "OK");
        }
    }

    private async void OnUpdateApiUrlClicked(object sender, EventArgs e)
    {
        var url = TxtApiUrl.Text?.Trim();
        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            await DisplayAlert("Validation", "Please enter a valid URL (including http:// or https://).", "OK");
            return;
        }

        _apiService.BaseUrl = url;
        await DisplayAlert("Saved", $"API Base URL updated to: {_apiService.BaseUrl}", "OK");
        await LoadSettingsAsync();
    }

    private async void OnSaveSettingsClicked(object sender, EventArgs e)
    {
        _ = decimal.TryParse(TxtMonthBudget.Text?.Trim(), out var monthBudget);
        _ = decimal.TryParse(TxtShopBudget.Text?.Trim(), out var shopBudget);
        _ = decimal.TryParse(TxtListBudget.Text?.Trim(), out var listBudget);

        var currency = PickerCurrency.SelectedIndex >= 0 ? (string)PickerCurrency.SelectedItem : "₹";

        var req = new UpdateSettingsRequest
        {
            Currency = currency,
            MonthBudget = monthBudget,
            ShopBudget = shopBudget,
            ListBudget = listBudget
        };

        var res = await _apiService.UpdateSettingsAsync(req);
        if (res != null && res.Success)
        {
            await DisplayAlert("Success", "Settings updated successfully.", "OK");
        }
        else
        {
            await DisplayAlert("Error", res?.Message ?? "Failed to save settings.", "OK");
        }
    }

    private async void OnLoadDemoClicked(object sender, EventArgs e)
    {
        var res = await _apiService.LoadDemoDataAsync();
        if (res != null && res.Success)
        {
            await DisplayAlert("Demo Data", "Sample purchases and transactions loaded successfully!", "OK");
        }
        else
        {
            await DisplayAlert("Notice", res?.Message ?? "Could not load demo data.", "OK");
        }
    }

    private async void OnClearDemoClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Remove Demo Data?", "Only records marked as Demo will be removed.", "Remove", "Cancel");
        if (confirm)
        {
            var res = await _apiService.ClearDemoDataAsync();
            if (res != null && res.Success)
            {
                await DisplayAlert("Success", "Demo data removed successfully.", "OK");
            }
            else
            {
                await DisplayAlert("Notice", res?.Message ?? "Could not remove demo data.", "OK");
            }
        }
    }

    private async void OnResetClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Wipe All Records?", "This will permanently erase all transactions, purchases, shops, and shopping items. Are you sure?", "Wipe All", "Cancel");
        if (confirm)
        {
            var res = await _apiService.ResetDatabaseAsync();
            if (res != null && res.Success)
            {
                await DisplayAlert("Reset", "Ledger cleared successfully.", "OK");
            }
            else
            {
                await DisplayAlert("Error", res?.Message ?? "Could not reset ledger.", "OK");
            }
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Sign Out?", "You will be returned to the login screen.", "Sign Out", "Cancel");
        if (confirm)
        {
            _authManager.Logout();
            Application.Current!.MainPage = new NavigationPage(new LoginPage(new ViewModels.LoginViewModel(_apiService, _authManager)));
        }
    }
}
