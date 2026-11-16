using Expense_Tracker.Services;
using Expense_Tracker.ViewModels;
using Expense_Tracker.Views;
using Microsoft.Extensions.Logging;

namespace Expense_Tracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services
            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddSingleton<IAuthManager, AuthManager>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<TransactionsPage>();
            builder.Services.AddTransient<AddTransactionPage>();
            builder.Services.AddTransient<SmartShoppingPage>();
            builder.Services.AddTransient<ShoppingListPage>();
            builder.Services.AddTransient<PriceComparePage>();
            builder.Services.AddTransient<SavingsPage>();
            builder.Services.AddTransient<CategoriesPage>();
            builder.Services.AddTransient<ShopsPage>();
            builder.Services.AddTransient<SettingsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
