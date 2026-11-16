using Expense_Tracker.Models;
using Expense_Tracker.Services;

namespace Expense_Tracker.Views;

public partial class SavingsPage : ContentPage
{
    private readonly IApiService _apiService;

    public SavingsPage() : this(new ApiService())
    {
    }

    public SavingsPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSavingsDataAsync();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadSavingsDataAsync();
        Refresher.IsRefreshing = false;
    }

    private async Task LoadSavingsDataAsync()
    {
        try
        {
            var res = await _apiService.GetSavingsSummaryAsync();
            if (res != null && res.Success && res.Data != null)
            {
                var d = res.Data;
                LblActSavings.Text = d.FormattedActualSavings;
                LblPotSavings.Text = d.FormattedPotentialSavings;
                LblListSavings.Text = d.FormattedListSavings;
                LblListActSavings.Text = d.FormattedListActualSavings;

                RenderActualLog(d.ActualSavingsLog);
                RenderMonthlyTrend(d.MonthlyActualSavings);
                RenderPotentialLog(d.HighestPotentialSavings);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not load savings: {ex.Message}", "OK");
        }
    }

    private void RenderActualLog(List<ActualSavingsLogItem> logItems)
    {
        LayoutActualLog.Children.Clear();
        if (logItems == null || !logItems.Any())
        {
            LayoutActualLog.Children.Add(new Label
            {
                Text = "Buy a product at a shop other than your usual one, at a lower price, to see actual savings.",
                TextColor = (Color)Application.Current!.Resources["TextDim"],
                FontSize = 12
            });
            return;
        }

        foreach (var item in logItems)
        {
            var border = new Border
            {
                BackgroundColor = (Color)Application.Current!.Resources["SurfaceCard"],
                Stroke = (Color)Application.Current.Resources["Line"],
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                Padding = new Thickness(14, 10)
            };

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var left = new VerticalStackLayout
            {
                Spacing = 2,
                Children =
                {
                    new Label { Text = item.SummaryText, TextColor = (Color)Application.Current.Resources["TextMain"], FontSize = 13, FontAttributes = FontAttributes.Bold },
                    new Label { Text = item.DetailsText, TextColor = (Color)Application.Current.Resources["TextDim"], FontSize = 11 }
                }
            };
            Grid.SetColumn(left, 0);

            var savedLabel = new Label
            {
                Text = item.FormattedSaved,
                TextColor = (Color)Application.Current.Resources["Positive"],
                FontAttributes = FontAttributes.Bold,
                FontSize = 13,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(savedLabel, 1);

            grid.Children.Add(left);
            grid.Children.Add(savedLabel);

            border.Content = grid;
            LayoutActualLog.Children.Add(border);
        }
    }

    private void RenderMonthlyTrend(List<MonthlySavingsTrendItem> trendItems)
    {
        LayoutMonthlySavings.Children.Clear();
        if (trendItems == null || !trendItems.Any())
        {
            LayoutMonthlySavings.Children.Add(new Label
            {
                Text = "No monthly savings recorded yet.",
                TextColor = (Color)Application.Current!.Resources["TextDim"],
                FontSize = 12
            });
            return;
        }

        foreach (var m in trendItems)
        {
            var stack = new VerticalStackLayout { Spacing = 4 };
            var hGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var title = new Label { Text = m.MonthName, TextColor = (Color)Application.Current!.Resources["TextMain"], FontSize = 13 };
            var amt = new Label { Text = m.FormattedAmount, TextColor = (Color)Application.Current.Resources["Positive"], FontAttributes = FontAttributes.Bold, FontSize = 13 };

            Grid.SetColumn(title, 0);
            Grid.SetColumn(amt, 1);
            hGrid.Children.Add(title);
            hGrid.Children.Add(amt);

            var bar = new ProgressBar
            {
                Progress = m.ProgressFraction,
                ProgressColor = (Color)Application.Current.Resources["AccentPrimary"],
                HeightRequest = 6
            };

            stack.Children.Add(hGrid);
            stack.Children.Add(bar);

            LayoutMonthlySavings.Children.Add(stack);
        }
    }

    private void RenderPotentialLog(List<PotentialSavingsItem> potItems)
    {
        LayoutPotentialLog.Children.Clear();
        if (potItems == null || !potItems.Any())
        {
            LayoutPotentialLog.Children.Add(new Label
            {
                Text = "No comparison data. Record prices from at least two shops for a product.",
                TextColor = (Color)Application.Current!.Resources["TextDim"],
                FontSize = 12
            });
            return;
        }

        foreach (var item in potItems)
        {
            var border = new Border
            {
                BackgroundColor = (Color)Application.Current!.Resources["SurfaceCard"],
                Stroke = (Color)Application.Current.Resources["Line"],
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                Padding = new Thickness(14, 10)
            };

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var left = new VerticalStackLayout
            {
                Spacing = 2,
                Children =
                {
                    new Label { Text = item.ProductName, TextColor = (Color)Application.Current.Resources["TextMain"], FontSize = 13, FontAttributes = FontAttributes.Bold },
                    new Label { Text = item.DetailsText, TextColor = (Color)Application.Current.Resources["TextDim"], FontSize = 11 }
                }
            };
            Grid.SetColumn(left, 0);

            var saveLabel = new Label
            {
                Text = item.FormattedSaving,
                TextColor = (Color)Application.Current.Resources["Positive"],
                FontAttributes = FontAttributes.Bold,
                FontSize = 13,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(saveLabel, 1);

            grid.Children.Add(left);
            grid.Children.Add(saveLabel);

            border.Content = grid;
            LayoutPotentialLog.Children.Add(border);
        }
    }
}
