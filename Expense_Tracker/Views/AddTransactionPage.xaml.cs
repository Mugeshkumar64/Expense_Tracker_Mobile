using Expense_Tracker.Models;
using Expense_Tracker.Services;

namespace Expense_Tracker.Views;

public partial class AddTransactionPage : ContentPage
{
    private readonly IApiService _apiService;
    private string _currentType = "expense";
    private List<Category> _allCategories = new();

    public AddTransactionPage() : this(new ApiService())
    {
    }

    public AddTransactionPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
        PickerDate.Date = DateTime.Now;
        PickerPayment.SelectedIndex = 0; // UPI
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var res = await _apiService.GetCategoriesAsync();
            if (res != null && res.Success && res.Data != null)
            {
                _allCategories = res.Data;
                UpdateCategoryPicker();
            }
        }
        catch
        {
            // Ignore error
        }
    }

    private void UpdateCategoryPicker()
    {
        PickerCategory.Items.Clear();
        var filtered = _allCategories
            .Where(c => c.Type.Equals(_currentType, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var c in filtered)
        {
            PickerCategory.Items.Add(c.Name);
        }

        if (PickerCategory.Items.Count > 0)
        {
            PickerCategory.SelectedIndex = 0;
        }
    }

    private void OnTypeExpenseClicked(object sender, EventArgs e)
    {
        _currentType = "expense";
        BtnTypeExpense.BackgroundColor = (Color)Application.Current!.Resources["Negative"];
        BtnTypeExpense.TextColor = Colors.White;
        BtnTypeIncome.BackgroundColor = Colors.Transparent;
        BtnTypeIncome.TextColor = (Color)Application.Current.Resources["TextSub"];
        UpdateCategoryPicker();
    }

    private void OnTypeIncomeClicked(object sender, EventArgs e)
    {
        _currentType = "income";
        BtnTypeIncome.BackgroundColor = (Color)Application.Current!.Resources["Positive"];
        BtnTypeIncome.TextColor = Colors.White;
        BtnTypeExpense.BackgroundColor = Colors.Transparent;
        BtnTypeExpense.TextColor = (Color)Application.Current.Resources["TextSub"];
        UpdateCategoryPicker();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var title = TxtTitle.Text?.Trim() ?? string.Empty;
        var amountText = TxtAmount.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title) || title.Length < 2)
        {
            await DisplayAlert("Validation", "Please enter a valid title (at least 2 characters).", "OK");
            return;
        }

        if (!decimal.TryParse(amountText, out var amount) || amount <= 0)
        {
            await DisplayAlert("Validation", "Please enter a valid positive amount.", "OK");
            return;
        }

        string? category = PickerCategory.SelectedIndex >= 0 ? PickerCategory.Items[PickerCategory.SelectedIndex] : null;
        if (string.IsNullOrWhiteSpace(category))
        {
            await DisplayAlert("Validation", "Please select a category.", "OK");
            return;
        }

        var payment = PickerPayment.SelectedIndex >= 0 ? (string)PickerPayment.SelectedItem : "UPI";

        BtnSave.IsEnabled = false;

        try
        {
            var req = new CreateTransactionRequest
            {
                Title = title,
                Amount = amount,
                Type = _currentType,
                CategoryName = category,
                Date = PickerDate.Date,
                PaymentMethod = payment,
                Notes = TxtNotes.Text?.Trim()
            };

            var res = await _apiService.CreateTransactionAsync(req);
            if (res != null && res.Success)
            {
                await DisplayAlert("Success", "Transaction recorded successfully.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", res?.Message ?? "Failed to save transaction.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not save transaction: {ex.Message}", "OK");
        }
        finally
        {
            BtnSave.IsEnabled = true;
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
