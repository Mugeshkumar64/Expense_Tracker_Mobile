using Expense_Tracker.Services;
using Expense_Tracker.ViewModels;
using Expense_Tracker.Views;

namespace Expense_Tracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var authManager = new AuthManager();
            if (authManager.IsAuthenticated)
            {
                MainPage = new AppShell();
            }
            else
            {
                var apiService = new ApiService();
                var loginViewModel = new LoginViewModel(apiService, authManager);
                MainPage = new NavigationPage(new LoginPage(loginViewModel));
            }
        }
    }
}
