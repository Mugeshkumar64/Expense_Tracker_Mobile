using Expense_Tracker.Models;
using Expense_Tracker.Services;

namespace Expense_Tracker.Views;

public partial class ShopsPage : ContentPage
{
    private readonly IApiService _apiService;

    public ShopsPage() : this(new ApiService())
    {
    }

    public ShopsPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadShopsAsync();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadShopsAsync();
        Refresher.IsRefreshing = false;
    }

    private async Task LoadShopsAsync()
    {
        try
        {
            var res = await _apiService.GetShopsAsync();
            if (res != null && res.Success && res.Data != null)
            {
                RenderShops(res.Data);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not load shops: {ex.Message}", "OK");
        }
    }

    private void RenderShops(List<Shop> shops)
    {
        LayoutShops.Children.Clear();
        if (shops == null || !shops.Any())
        {
            LayoutShops.Children.Add(new Label
            {
                Text = "No shops registered yet. Shops are added automatically when you record purchases, or add one above.",
                TextColor = (Color)Application.Current!.Resources["TextDim"],
                FontSize = 13,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(20)
            });
            return;
        }

        foreach (var sh in shops)
        {
            var border = new Border
            {
                BackgroundColor = (Color)Application.Current!.Resources["SurfaceCard"],
                Stroke = (Color)Application.Current.Resources["Line"],
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 },
                Padding = new Thickness(16, 12)
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

            var left = new VerticalStackLayout
            {
                Spacing = 2,
                Children =
                {
                    new Label { Text = sh.Name, TextColor = (Color)Application.Current.Resources["TextMain"], FontSize = 15, FontAttributes = FontAttributes.Bold },
                    new Label { Text = $"📍 {sh.LocationDisplay}", TextColor = (Color)Application.Current.Resources["TextDim"], FontSize = 11 }
                }
            };
            Grid.SetColumn(left, 0);

            var stats = new VerticalStackLayout
            {
                Spacing = 2,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label { Text = sh.FormattedTotalSpending, TextColor = (Color)Application.Current.Resources["TextMain"], FontAttributes = FontAttributes.Bold, FontSize = 13, HorizontalOptions = LayoutOptions.End },
                    new Label { Text = $"{sh.ProductsCount} products · {sh.LowestMatchesText}", TextColor = (Color)Application.Current.Resources["Positive"], FontSize = 10, HorizontalOptions = LayoutOptions.End }
                }
            };
            Grid.SetColumn(stats, 1);

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
                bool confirm = await DisplayAlert("Delete Shop?", $"Remove '{sh.Name}'?", "Delete", "Cancel");
                if (confirm)
                {
                    var res = await _apiService.DeleteShopAsync(sh.Id);
                    if (res != null && res.Success)
                    {
                        await LoadShopsAsync();
                    }
                    else
                    {
                        await DisplayAlert("Notice", res?.Message ?? "Could not remove shop.", "OK");
                    }
                }
            };
            Grid.SetColumn(deleteBtn, 2);

            grid.Children.Add(left);
            grid.Children.Add(stats);
            grid.Children.Add(deleteBtn);

            border.Content = grid;
            LayoutShops.Children.Add(border);
        }
    }

    private async void OnAddShopClicked(object sender, EventArgs e)
    {
        var name = TxtShopName.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
        {
            await DisplayAlert("Validation", "Please enter a shop name.", "OK");
            return;
        }

        var req = new CreateShopRequest
        {
            Name = name,
            Location = TxtShopLoc.Text?.Trim()
        };

        var res = await _apiService.CreateShopAsync(req);
        if (res != null && res.Success)
        {
            TxtShopName.Text = string.Empty;
            TxtShopLoc.Text = string.Empty;
            await LoadShopsAsync();
        }
        else
        {
            await DisplayAlert("Error", res?.Message ?? "Could not add shop.", "OK");
        }
    }
}
