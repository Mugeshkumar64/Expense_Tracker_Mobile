using Expense_Tracker.ViewModels;

namespace Expense_Tracker.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage() : this(new LoginViewModel())
    {
    }

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
