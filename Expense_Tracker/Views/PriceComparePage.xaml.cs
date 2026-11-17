using Expense_Tracker.Models;
using Expense_Tracker.Services;

namespace Expense_Tracker.Views;

public partial class PriceComparePage : ContentPage
{
    private readonly IApiService _apiService;

    public PriceComparePage() : this(new ApiService())
    {
    }

    public PriceComparePage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    private async void OnCompareClicked(object sender, EventArgs e)
    {
        var product = TxtSearchProduct.Text?.Trim();
        if (string.IsNullOrWhiteSpace(product))
        {
            await DisplayAlert("Validation", "Please enter a product name to compare.", "OK");
            return;
        }

        _ = decimal.TryParse(TxtCompareQty.Text?.Trim(), out var qty);
        if (qty <= 0) qty = 1;

        try
        {
            var res = await _apiService.ComparePricesAsync(product, qty);
            if (res != null && res.Success && res.Data != null)
            {
                RenderResults(res.Data);
            }
            else
            {
                await DisplayAlert("No Match", res?.Message ?? "No comparison data found for this product.", "OK");
                LayoutResults.IsVisible = false;
                LayoutEmpty.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not compare prices: {ex.Message}", "OK");
        }
    }

    private void RenderResults(PriceComparisonResult data)
    {
        LayoutEmpty.IsVisible = false;
        LayoutResults.IsVisible = true;

        LblLowestPrice.Text = data.FormattedLowestPrice;
        LblLowestDate.Text = $"as of {data.LowestRecordedDate:yyyy-MM-dd}";

        LblCheapestShop.Text = data.CheapestShop;
        LblShopsCompared.Text = $"{data.ShopRows.Count} shops compared";

        LblLastPaid.Text = data.FormattedLastPaid;
        LblLastShop.Text = $"{data.LastPurchaseShop} · {data.LastPurchaseDate:yyyy-MM-dd}";

        LblSavingVsLast.Text = $"₹{data.SavingVsLastPurchasePerUnit:N2}/{data.BaseUnit}";
        LblSavingTotal.Text = $"₹{data.TotalSavingVsLastPurchase:N2} on {data.PlannedQuantity:0.###} {data.BaseUnit}";

        LblSuggestion.Text = data.SuggestionText;

        // Render Shop Comparison Rows
        LayoutShopRows.Children.Clear();
        foreach (var row in data.ShopRows)
        {
            var border = new Border
            {
                BackgroundColor = Color.FromArgb(row.IsLowestPrice ? "#ECFDF5" : "#FFFFFF"),
                Stroke = row.IsLowestPrice ? (Color)Application.Current!.Resources["PositiveBorder"] : (Color)Application.Current!.Resources["Line"],
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

            var shopInfo = new VerticalStackLayout
            {
                Spacing = 2,
                Children =
                {
                    new Label { Text = row.ShopName, TextColor = (Color)Application.Current.Resources["TextMain"], FontSize = 14, FontAttributes = FontAttributes.Bold },
                    new Label { Text = $"Recorded: {row.FormattedDate}{(row.IsPotentiallyOutdated ? " (outdated)" : "")}", TextColor = (Color)Application.Current.Resources["TextDim"], FontSize = 11 }
                }
            };
            Grid.SetColumn(shopInfo, 0);

            var priceInfo = new VerticalStackLayout
            {
                Spacing = 2,
                HorizontalOptions = LayoutOptions.End,
                Children =
                {
                    new Label { Text = $"{row.FormattedPrice}/{data.BaseUnit}", TextColor = (Color)Application.Current.Resources["TextMain"], FontSize = 13, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End },
                    new Label { Text = $"Total: {row.FormattedTotal}", TextColor = (Color)Application.Current.Resources["TextDim"], FontSize = 11, HorizontalOptions = LayoutOptions.End }
                }
            };
            Grid.SetColumn(priceInfo, 1);

            var badge = new Label
            {
                Text = row.StatusBadge,
                TextColor = Color.FromArgb(row.StatusColor),
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(badge, 2);

            grid.Children.Add(shopInfo);
            grid.Children.Add(priceInfo);
            grid.Children.Add(badge);

            border.Content = grid;
            LayoutShopRows.Children.Add(border);
        }

        // Render Price History
        LayoutHistoryRows.Children.Clear();
        foreach (var h in data.PriceHistory)
        {
            var hGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                Padding = new Thickness(4, 4)
            };

            var label = new Label
            {
                Text = $"{h.FormattedDate} · {h.ShopName}",
                TextColor = (Color)Application.Current!.Resources["TextSub"],
                FontSize = 13,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(label, 0);

            var price = new Label
            {
                Text = h.FormattedPrice,
                TextColor = (Color)Application.Current.Resources["TextMain"],
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(price, 1);

            hGrid.Children.Add(label);
            hGrid.Children.Add(price);
            LayoutHistoryRows.Children.Add(hGrid);
        }
    }
}
