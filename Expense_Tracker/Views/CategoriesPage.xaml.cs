using Expense_Tracker.Models;
using Expense_Tracker.Services;

namespace Expense_Tracker.Views;

public partial class CategoriesPage : ContentPage
{
    private readonly IApiService _apiService;
    private string? _currentType = null;

    public CategoriesPage() : this(new ApiService())
    {
    }

    public CategoriesPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCategoriesAsync();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadCategoriesAsync();
        Refresher.IsRefreshing = false;
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var res = await _apiService.GetCategoriesAsync(type: _currentType);
            if (res != null && res.Success && res.Data != null)
            {
                CvCategories.ItemsSource = res.Data;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not load categories: {ex.Message}", "OK");
        }
    }

    private void OnFilterAllClicked(object sender, EventArgs e)
    {
        _currentType = null;
        UpdateButtonStyles(BtnFilterAll);
        _ = LoadCategoriesAsync();
    }

    private void OnFilterExpenseClicked(object sender, EventArgs e)
    {
        _currentType = "expense";
        UpdateButtonStyles(BtnFilterExpense);
        _ = LoadCategoriesAsync();
    }

    private void OnFilterIncomeClicked(object sender, EventArgs e)
    {
        _currentType = "income";
        UpdateButtonStyles(BtnFilterIncome);
        _ = LoadCategoriesAsync();
    }

    private void UpdateButtonStyles(Button activeButton)
    {
        var accent = (Color)Application.Current!.Resources["AccentPrimary"];
        var surface = (Color)Application.Current.Resources["SurfaceCard"];
        var textSub = (Color)Application.Current.Resources["TextSub"];

        BtnFilterAll.BackgroundColor = BtnFilterAll == activeButton ? accent : surface;
        BtnFilterAll.TextColor = BtnFilterAll == activeButton ? Colors.White : textSub;

        BtnFilterExpense.BackgroundColor = BtnFilterExpense == activeButton ? accent : surface;
        BtnFilterExpense.TextColor = BtnFilterExpense == activeButton ? Colors.White : textSub;

        BtnFilterIncome.BackgroundColor = BtnFilterIncome == activeButton ? accent : surface;
        BtnFilterIncome.TextColor = BtnFilterIncome == activeButton ? Colors.White : textSub;
    }

    private async void OnAddCategoryClicked(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("New Category", "Enter category name:");
        if (string.IsNullOrWhiteSpace(name)) return;

        string typeChoice = await DisplayActionSheet("Category Type", "Cancel", null, "Expense", "Income");
        if (typeChoice == "Cancel" || string.IsNullOrWhiteSpace(typeChoice)) return;

        var req = new CreateCategoryRequest
        {
            Name = name.Trim(),
            Type = typeChoice.ToLowerInvariant()
        };

        var res = await _apiService.CreateCategoryAsync(req);
        if (res != null && res.Success)
        {
            await LoadCategoriesAsync();
        }
        else
        {
            await DisplayAlert("Error", res?.Message ?? "Could not create category.", "OK");
        }
    }

    private async void OnDeleteCategoryClicked(object sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: Category cat })
        {
            bool confirm = await DisplayAlert("Delete Category?", $"Are you sure you want to remove '{cat.Name}'?", "Delete", "Cancel");
            if (confirm)
            {
                var res = await _apiService.DeleteCategoryAsync(cat.Id);
                if (res != null && res.Success)
                {
                    await LoadCategoriesAsync();
                }
                else
                {
                    await DisplayAlert("Notice", res?.Message ?? "Category cannot be deleted.", "OK");
                }
            }
        }
    }
}
