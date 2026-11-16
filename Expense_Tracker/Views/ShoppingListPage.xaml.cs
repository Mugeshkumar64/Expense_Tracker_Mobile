using Expense_Tracker.Models;
using Expense_Tracker.Services;

namespace Expense_Tracker.Views;

public partial class ShoppingListPage : ContentPage
{
    private readonly IApiService _apiService;

    public ShoppingListPage() : this(new ApiService())
    {
    }

    public ShoppingListPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
        PickerItemUnit.SelectedIndex = 0; // kg
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSummaryAsync();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadSummaryAsync();
        Refresher.IsRefreshing = false;
    }

    private async Task LoadSummaryAsync()
    {
        try
        {
            var res = await _apiService.GetShoppingListSummaryAsync();
            if (res != null && res.Success && res.Data != null)
            {
                var d = res.Data;
                LblEstimatedTotal.Text = d.FormattedEstimatedCost;
                LblEstimatedSavings.Text = d.FormattedEstimatedSavings;
                LblBudgetStatus.Text = d.BudgetStatusText;
                RenderItems(d.Items);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not load shopping list: {ex.Message}", "OK");
        }
    }

    private void RenderItems(List<ShoppingListItem> items)
    {
        LayoutItems.Children.Clear();
        if (items == null || !items.Any())
        {
            LayoutItems.Children.Add(new Label
            {
                Text = "Your list is empty. Add products you plan to buy.",
                TextColor = (Color)Application.Current!.Resources["TextDim"],
                FontSize = 13,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(20)
            });
            return;
        }

        foreach (var item in items)
        {
            var border = new Border
            {
                BackgroundColor = (Color)Application.Current!.Resources["SurfaceCard"],
                Stroke = (Color)Application.Current.Resources["Line"],
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 },
                Padding = new Thickness(14, 12)
            };

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                ColumnSpacing = 8
            };

            // Details
            var details = new VerticalStackLayout
            {
                Spacing = 2,
                Children =
                {
                    new Label { Text = item.ProductName, TextColor = (Color)Application.Current.Resources["TextMain"], FontSize = 14, FontAttributes = FontAttributes.Bold },
                    new Label { Text = $"{item.FormattedQty} · {item.PriceNote}", TextColor = (Color)Application.Current.Resources["TextDim"], FontSize = 11 }
                }
            };
            Grid.SetColumn(details, 0);

            // Estimated Cost
            var costLabel = new Label
            {
                Text = item.FormattedCost,
                TextColor = item.HasRecordedPrice ? (Color)Application.Current.Resources["TextMain"] : (Color)Application.Current.Resources["Gold"],
                FontAttributes = FontAttributes.Bold,
                FontSize = 13,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(costLabel, 1);

            // Buy Button
            var buyBtn = new Button
            {
                Text = "Buy",
                BackgroundColor = (Color)Application.Current.Resources["AccentPrimary"],
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 12,
                CornerRadius = 8,
                Padding = new Thickness(10, 0),
                HeightRequest = 32,
                VerticalOptions = LayoutOptions.Center
            };
            buyBtn.Clicked += async (s, e) =>
            {
                await BuyItemAsync(item);
            };
            Grid.SetColumn(buyBtn, 2);

            // Delete Button
            var deleteBtn = new Button
            {
                Text = "🗑",
                BackgroundColor = Colors.Transparent,
                TextColor = (Color)Application.Current.Resources["Negative"],
                FontSize = 14,
                Padding = new Thickness(4, 0),
                HeightRequest = 32,
                VerticalOptions = LayoutOptions.Center
            };
            deleteBtn.Clicked += async (s, e) =>
            {
                await _apiService.DeleteShoppingListItemAsync(item.Id);
                await LoadSummaryAsync();
            };
            Grid.SetColumn(deleteBtn, 3);

            grid.Children.Add(details);
            grid.Children.Add(costLabel);
            grid.Children.Add(buyBtn);
            grid.Children.Add(deleteBtn);

            border.Content = grid;
            LayoutItems.Children.Add(border);
        }
    }

    private async Task BuyItemAsync(ShoppingListItem item)
    {
        string shopPrompt = await DisplayPromptAsync("Buy Item", $"Enter shop name for {item.ProductName}:", initialValue: item.CheapestShop ?? "Local Supermart");
        if (string.IsNullOrWhiteSpace(shopPrompt)) return;

        string pricePrompt = await DisplayPromptAsync("Unit Price", $"Enter unit price per {item.Unit}:", initialValue: item.CheapestPricePerBaseUnit?.ToString("0.00") ?? "50.00", keyboard: Keyboard.Numeric);
        if (!decimal.TryParse(pricePrompt, out var unitPrice) || unitPrice <= 0) return;

        var purchaseReq = new CreatePurchaseRequest
        {
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            Unit = item.Unit,
            UnitPrice = unitPrice,
            ShopName = shopPrompt.Trim(),
            Date = DateTime.Now,
            FromList = true
        };

        var res = await _apiService.BuyShoppingListItemAsync(item.Id, purchaseReq);
        if (res != null && res.Success)
        {
            await DisplayAlert("Success", $"Purchase recorded for '{item.ProductName}' and linked to transactions!", "OK");
            await LoadSummaryAsync();
        }
        else
        {
            await DisplayAlert("Error", res?.Message ?? "Failed to convert item to purchase.", "OK");
        }
    }

    private async void OnAddItemClicked(object sender, EventArgs e)
    {
        var product = TxtItemName.Text?.Trim() ?? string.Empty;
        var qtyText = TxtItemQty.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(product) || product.Length < 2)
        {
            await DisplayAlert("Validation", "Please enter a product name.", "OK");
            return;
        }

        if (!decimal.TryParse(qtyText, out var qty) || qty <= 0)
        {
            await DisplayAlert("Validation", "Please enter a valid quantity.", "OK");
            return;
        }

        var unit = PickerItemUnit.SelectedIndex >= 0 ? (string)PickerItemUnit.SelectedItem : "kg";

        var req = new CreateShoppingListItemRequest
        {
            ProductName = product,
            Quantity = qty,
            Unit = unit
        };

        var res = await _apiService.AddShoppingListItemAsync(req);
        if (res != null && res.Success)
        {
            TxtItemName.Text = string.Empty;
            await LoadSummaryAsync();
        }
        else
        {
            await DisplayAlert("Error", res?.Message ?? "Failed to add item.", "OK");
        }
    }
}
