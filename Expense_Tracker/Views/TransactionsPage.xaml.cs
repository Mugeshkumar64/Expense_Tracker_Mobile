using Expense_Tracker.Models;
using Expense_Tracker.Services;

namespace Expense_Tracker.Views;

public partial class TransactionsPage : ContentPage
{
    private readonly IApiService _apiService;
    private string _currentType = "all";
    private string? _currentCategory = null;
    private string? _currentSearch = null;
    private List<Category> _categories = new();

    public TransactionsPage() : this(new ApiService())
    {
    }

    public TransactionsPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCategoriesAsync();
        await LoadTransactionsAsync();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadTransactionsAsync();
        Refresher.IsRefreshing = false;
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var res = await _apiService.GetCategoriesAsync();
            if (res != null && res.Success && res.Data != null)
            {
                _categories = res.Data;
                PickerCategory.Items.Clear();
                PickerCategory.Items.Add("All Categories");
                foreach (var c in _categories.Select(x => x.Name).Distinct())
                {
                    PickerCategory.Items.Add(c);
                }
                PickerCategory.SelectedIndex = 0;
            }
        }
        catch
        {
            // Ignore category loading error in UI
        }
    }

    private async Task LoadTransactionsAsync()
    {
        try
        {
            var typeParam = _currentType == "all" ? null : _currentType;
            var catParam = (_currentCategory == "all" || _currentCategory == "All Categories") ? null : _currentCategory;

            var res = await _apiService.GetTransactionsAsync(
                type: typeParam,
                category: catParam,
                search: _currentSearch);

            if (res != null && res.Success && res.Data != null)
            {
                var list = res.Data;
                CvTransactions.ItemsSource = list;
                var incTotal = list.Where(x => x.IsIncome).Sum(x => x.Amount);
                var expTotal = list.Where(x => !x.IsIncome).Sum(x => x.Amount);
                var net = incTotal - expTotal;
                LblTxCount.Text = $"{list.Count} entries";
                LblTxIncome.Text = $"₹{incTotal:N0}";
                LblTxExpense.Text = $"₹{expTotal:N0}";
                LblNetFlow.Text = (net >= 0 ? "+₹" : "−₹") + $"{Math.Abs(net):N0}";
                LblNetFlow.TextColor = net >= 0 ? Color.FromArgb("#6EE7B7") : Color.FromArgb("#FDA4AF");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not load transactions: {ex.Message}", "OK");
        }
    }

    private void OnFilterAllClicked(object sender, EventArgs e)
    {
        _currentType = "all";
        UpdateButtonStyles(BtnFilterAll);
        _ = LoadTransactionsAsync();
    }

    private void OnFilterIncomeClicked(object sender, EventArgs e)
    {
        _currentType = "income";
        UpdateButtonStyles(BtnFilterIncome);
        _ = LoadTransactionsAsync();
    }

    private void OnFilterExpenseClicked(object sender, EventArgs e)
    {
        _currentType = "expense";
        UpdateButtonStyles(BtnFilterExpense);
        _ = LoadTransactionsAsync();
    }

    private void UpdateButtonStyles(Button activeButton)
    {
        var accent = (Color)Application.Current!.Resources["AccentPrimary"];
        var surface = (Color)Application.Current.Resources["SurfaceCard"];
        var textSub = (Color)Application.Current.Resources["TextSub"];

        BtnFilterAll.BackgroundColor = BtnFilterAll == activeButton ? accent : surface;
        BtnFilterAll.TextColor = BtnFilterAll == activeButton ? Colors.White : textSub;

        BtnFilterIncome.BackgroundColor = BtnFilterIncome == activeButton ? accent : surface;
        BtnFilterIncome.TextColor = BtnFilterIncome == activeButton ? Colors.White : textSub;

        BtnFilterExpense.BackgroundColor = BtnFilterExpense == activeButton ? accent : surface;
        BtnFilterExpense.TextColor = BtnFilterExpense == activeButton ? Colors.White : textSub;
    }

    private void OnCategorySelected(object sender, EventArgs e)
    {
        if (PickerCategory.SelectedIndex >= 0)
        {
            _currentCategory = PickerCategory.Items[PickerCategory.SelectedIndex];
            _ = LoadTransactionsAsync();
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        _currentSearch = e.NewTextValue?.Trim();
        _ = LoadTransactionsAsync();
    }

    private async void OnDeleteTransactionInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem { BindingContext: Transaction item })
        {
            bool confirm = await DisplayAlert("Delete Transaction?", $"Are you sure you want to delete '{item.Title}'?", "Delete", "Cancel");
            if (confirm)
            {
                var res = await _apiService.DeleteTransactionAsync(item.Id);
                if (res != null && res.Success)
                {
                    await LoadTransactionsAsync();
                }
                else
                {
                    await DisplayAlert("Error", res?.Message ?? "Failed to delete transaction.", "OK");
                }
            }
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddTransactionPage(_apiService));
    }
}
