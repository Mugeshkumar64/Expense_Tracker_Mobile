using Expense_Tracker.Models;
using Expense_Tracker.Services;

namespace Expense_Tracker.Views;

public partial class SmartShoppingPage : ContentPage
{
    private readonly IApiService _apiService;

    public SmartShoppingPage() : this(new ApiService())
    {
    }

    public SmartShoppingPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
        PickerUnit.SelectedIndex = 0; // kg
        PickerPayment.SelectedIndex = 0; // UPI
        PickerDate.Date = DateTime.Now;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCategoriesAsync();
        await LoadPurchasesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var res = await _apiService.GetCategoriesAsync(type: "expense");
            if (res != null && res.Success && res.Data != null)
            {
                PickerCategory.Items.Clear();
                foreach (var c in res.Data)
                {
                    PickerCategory.Items.Add(c.Name);
                }

                int groceriesIdx = PickerCategory.Items.IndexOf("Groceries");
                PickerCategory.SelectedIndex = groceriesIdx >= 0 ? groceriesIdx : (PickerCategory.Items.Count > 0 ? 0 : -1);
            }
        }
        catch
        {
            // Ignore error
        }
    }

    private void OnPriceOrQtyChanged(object sender, TextChangedEventArgs e)
    {
        decimal qty = 0;
        decimal price = 0;

        _ = decimal.TryParse(TxtQuantity.Text?.Trim(), out qty);
        _ = decimal.TryParse(TxtUnitPrice.Text?.Trim(), out price);

        if (qty > 0 && price >= 0)
        {
            var total = Math.Round(qty * price, 2);
            LblTotalCalculated.Text = $"₹{total:N2}";
        }
        else
        {
            LblTotalCalculated.Text = "₹0.00";
        }
    }

    private async Task LoadPurchasesAsync()
    {
        try
        {
            var res = await _apiService.GetPurchasesAsync();
            if (res != null && res.Success && res.Data != null)
            {
                RenderPurchases(res.Data);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not load purchases: {ex.Message}", "OK");
        }
    }

    private void RenderPurchases(List<Purchase> purchases)
    {
        LayoutPurchases.Children.Clear();
        if (!purchases.Any())
        {
            LayoutPurchases.Children.Add(new Label
            {
                Text = "No purchases recorded yet.",
                TextColor = (Color)Application.Current!.Resources["TextDim"],
                FontSize = 13,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(20)
            });
            return;
        }

        foreach (var p in purchases)
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
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                ColumnSpacing = 10
            };

            // Details
            var info = new VerticalStackLayout
            {
                Spacing = 2,
                Children =
                {
                    new Label { Text = p.ProductName, TextColor = (Color)Application.Current.Resources["TextMain"], FontSize = 14, FontAttributes = FontAttributes.Bold },
                    new Label { Text = p.Subtitle, TextColor = (Color)Application.Current.Resources["TextDim"], FontSize = 11 }
                }
            };
            Grid.SetColumn(info, 0);

            // Total
            var totalLabel = new Label
            {
                Text = p.FormattedTotal,
                TextColor = (Color)Application.Current.Resources["AccentPrimary"],
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(totalLabel, 1);

            // Delete button
            var deleteBtn = new Button
            {
                Text = "🗑",
                BackgroundColor = Colors.Transparent,
                TextColor = (Color)Application.Current.Resources["Negative"],
                FontSize = 14,
                Padding = new Thickness(8, 0),
                HeightRequest = 36,
                VerticalOptions = LayoutOptions.Center
            };
            deleteBtn.Clicked += async (s, e) =>
            {
                bool confirm = await DisplayAlert("Delete Purchase?", $"Delete purchase for '{p.ProductName}' and its linked expense?", "Delete", "Cancel");
                if (confirm)
                {
                    await _apiService.DeletePurchaseAsync(p.Id);
                    await LoadPurchasesAsync();
                }
            };
            Grid.SetColumn(deleteBtn, 2);

            grid.Children.Add(info);
            grid.Children.Add(totalLabel);
            grid.Children.Add(deleteBtn);

            border.Content = grid;
            LayoutPurchases.Children.Add(border);
        }
    }

    private async void OnSavePurchaseClicked(object sender, EventArgs e)
    {
        var product = TxtProductName.Text?.Trim() ?? string.Empty;
        var shop = TxtShopName.Text?.Trim() ?? string.Empty;
        var qtyText = TxtQuantity.Text?.Trim() ?? string.Empty;
        var priceText = TxtUnitPrice.Text?.Trim() ?? string.Empty;

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

        if (!decimal.TryParse(priceText, out var unitPrice) || unitPrice <= 0)
        {
            await DisplayAlert("Validation", "Please enter a valid unit price.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(shop) || shop.Length < 2)
        {
            await DisplayAlert("Validation", "Please enter a shop name.", "OK");
            return;
        }

        var unit = PickerUnit.SelectedIndex >= 0 ? (string)PickerUnit.SelectedItem : "kg";
        var category = PickerCategory.SelectedIndex >= 0 ? PickerCategory.Items[PickerCategory.SelectedIndex] : "Groceries";
        var payment = PickerPayment.SelectedIndex >= 0 ? (string)PickerPayment.SelectedItem : "UPI";

        BtnSavePurchase.IsEnabled = false;

        try
        {
            var req = new CreatePurchaseRequest
            {
                ProductName = product,
                CategoryName = category,
                Quantity = qty,
                Unit = unit,
                UnitPrice = unitPrice,
                ShopName = shop,
                ShopLocation = TxtShopLocation.Text?.Trim(),
                PaymentMethod = payment,
                Date = PickerDate.Date
            };

            var res = await _apiService.CreatePurchaseAsync(req);
            if (res != null && res.Success)
            {
                await DisplayAlert("Success", "Purchase saved! A linked expense transaction was automatically created.", "OK");
                TxtProductName.Text = string.Empty;
                TxtUnitPrice.Text = string.Empty;
                TxtShopName.Text = string.Empty;
                TxtShopLocation.Text = string.Empty;
                await LoadPurchasesAsync();
            }
            else
            {
                await DisplayAlert("Error", res?.Message ?? "Failed to save purchase.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not save purchase: {ex.Message}", "OK");
        }
        finally
        {
            BtnSavePurchase.IsEnabled = true;
        }
    }
}
