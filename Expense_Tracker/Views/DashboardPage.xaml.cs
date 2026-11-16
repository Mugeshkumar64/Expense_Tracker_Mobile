using Expense_Tracker.Models;
using Expense_Tracker.Services;

namespace Expense_Tracker.Views;

public partial class DashboardPage : ContentPage
{
    private readonly IApiService _apiService;
    private readonly IAuthManager _authManager;

    public DashboardPage() : this(new ApiService(), new AuthManager())
    {
    }

    public DashboardPage(IApiService apiService, IAuthManager authManager)
    {
        InitializeComponent();
        _apiService = apiService;
        _authManager = authManager;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        UpdateUserInfo();
        await LoadDashboardDataAsync();
    }

    private void UpdateUserInfo()
    {
        var user = _authManager.CurrentUser;
        if (user != null)
        {
            LblUserName.Text = user.FullName;
            LblAvatar.Text = user.Initials;
        }
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadDashboardDataAsync();
        Refresher.IsRefreshing = false;
    }

    private async Task LoadDashboardDataAsync()
    {
        try
        {
            var res = await _apiService.GetDashboardSummaryAsync();
            if (res != null && res.Success && res.Data != null)
            {
                var d = res.Data;

                LblTotalBalance.Text = d.FormattedBalance;
                LblIncome.Text = d.FormattedIncome;
                LblExpense.Text = d.FormattedExpense;
                LblSavings.Text = d.FormattedSavings;
                LblSavingsPct.Text = d.FormattedSavingsPct;

                LblShopLeft.Text = d.FormattedShoppingBudgetLeft;
                LblShopNote.Text = d.HasShoppingBudget ? $"₹{d.ShoppingSpentThisMonth:N0} spent on purchases" : "Set in Settings";

                LblBudgetUsed.Text = d.FormattedBudgetUsed;

                LblPotSavings.Text = d.FormattedPotentialSavings;
                LblActSavings.Text = d.FormattedActualSavings;

                RenderInsights(d.SmartInsights);
                RenderMovers(d.PriceMovers, d.CheapestShops);
                RenderRecentTransactions(d.RecentTransactions);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not load dashboard: {ex.Message}", "OK");
        }
    }

    private void RenderInsights(List<string> insights)
    {
        LayoutInsights.Children.Clear();
        if (insights == null || !insights.Any())
        {
            LayoutInsights.Children.Add(new Label
            {
                Text = "Not enough data yet. Insights appear once you record purchases and expenses.",
                TextColor = (Color)Application.Current!.Resources["TextDim"],
                FontSize = 12
            });
            return;
        }

        foreach (var text in insights)
        {
            var border = new Border
            {
                BackgroundColor = Color.FromArgb("#162032"),
                Stroke = (Color)Application.Current!.Resources["Line"],
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                Padding = new Thickness(12, 10),
                Content = new Label
                {
                    Text = "• " + text,
                    TextColor = (Color)Application.Current.Resources["TextMain"],
                    FontSize = 13,
                    LineHeight = 1.3
                }
            };
            LayoutInsights.Children.Add(border);
        }
    }

    private void RenderMovers(List<PriceMoverItem> movers, List<CheapestShopItem> cheapestShops)
    {
        LayoutMovers.Children.Clear();
        bool hasContent = false;

        if (movers != null && movers.Any())
        {
            hasContent = true;
            foreach (var m in movers)
            {
                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Auto }
                    },
                    Padding = new Thickness(0, 4)
                };

                var label = new Label
                {
                    Text = m.PriceSummary,
                    TextColor = (Color)Application.Current!.Resources["TextMain"],
                    FontSize = 13,
                    VerticalOptions = LayoutOptions.Center
                };
                Grid.SetColumn(label, 0);

                var badge = new Label
                {
                    Text = m.FormattedChange,
                    TextColor = Color.FromArgb(m.ChangeColor),
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 12,
                    VerticalOptions = LayoutOptions.Center
                };
                Grid.SetColumn(badge, 1);

                grid.Children.Add(label);
                grid.Children.Add(badge);
                LayoutMovers.Children.Add(grid);
            }
        }

        if (cheapestShops != null && cheapestShops.Any())
        {
            hasContent = true;
            var subheader = new Label
            {
                Text = "Cheapest recorded shop (frequent products)",
                TextColor = (Color)Application.Current!.Resources["TextDim"],
                FontSize = 11,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(0, 8, 0, 4)
            };
            LayoutMovers.Children.Add(subheader);

            foreach (var c in cheapestShops)
            {
                LayoutMovers.Children.Add(new Label
                {
                    Text = "✓ " + c.DisplayText,
                    TextColor = (Color)Application.Current.Resources["TextMain"],
                    FontSize = 12,
                    Margin = new Thickness(4, 2)
                });
            }
        }

        if (!hasContent)
        {
            LayoutMovers.Children.Add(new Label
            {
                Text = "No price history yet. Repeat purchases of a product reveal price changes.",
                TextColor = (Color)Application.Current!.Resources["TextDim"],
                FontSize = 12
            });
        }
    }

    private void RenderRecentTransactions(List<Transaction> transactions)
    {
        LayoutRecentTx.Children.Clear();
        if (transactions == null || !transactions.Any())
        {
            LayoutRecentTx.Children.Add(new Label
            {
                Text = "No records registered yet.",
                TextColor = (Color)Application.Current!.Resources["TextDim"],
                FontSize = 13,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(20)
            });
            return;
        }

        foreach (var t in transactions)
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
                    new ColumnDefinition { Width = new GridLength(40) },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                ColumnSpacing = 12
            };

            // Icon
            var iconBorder = new Border
            {
                WidthRequest = 36,
                HeightRequest = 36,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                BackgroundColor = Color.FromArgb(t.IsIncome ? "#122822" : "#2A1820"),
                StrokeThickness = 0,
                Content = new Label
                {
                    Text = t.TypeIcon,
                    TextColor = Color.FromArgb(t.AmountColor),
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };
            Grid.SetColumn(iconBorder, 0);

            // Title & Subtitle
            var infoStack = new VerticalStackLayout
            {
                Spacing = 2,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label { Text = t.Title, TextColor = (Color)Application.Current.Resources["TextMain"], FontSize = 14, FontAttributes = FontAttributes.Bold, LineBreakMode = LineBreakMode.TailTruncation },
                    new Label { Text = t.Subtitle, TextColor = (Color)Application.Current.Resources["TextDim"], FontSize = 11 }
                }
            };
            Grid.SetColumn(infoStack, 1);

            // Amount
            var amountLabel = new Label
            {
                Text = t.FormattedAmount,
                TextColor = Color.FromArgb(t.AmountColor),
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(amountLabel, 2);

            grid.Children.Add(iconBorder);
            grid.Children.Add(infoStack);
            grid.Children.Add(amountLabel);

            border.Content = grid;
            LayoutRecentTx.Children.Add(border);
        }
    }

    private async void OnViewAllClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//transactions");
    }
}
