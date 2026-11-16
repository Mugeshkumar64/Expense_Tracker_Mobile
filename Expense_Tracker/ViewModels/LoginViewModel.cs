using System.Windows.Input;
using Expense_Tracker.Models;
using Expense_Tracker.Services;

namespace Expense_Tracker.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly IAuthManager _authManager;

    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _fullName = string.Empty;
    private bool _isRegisterMode = false;
    private string _errorMessage = string.Empty;

    public string Email
    {
        get => _email;
        set
        {
            if (SetProperty(ref _email, value))
            {
                ClearError();
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
            {
                ClearError();
            }
        }
    }

    public string FullName
    {
        get => _fullName;
        set
        {
            if (SetProperty(ref _fullName, value))
            {
                ClearError();
            }
        }
    }

    public bool IsRegisterMode
    {
        get => _isRegisterMode;
        set
        {
            if (SetProperty(ref _isRegisterMode, value))
            {
                OnPropertyChanged(nameof(IsSignInMode));
                OnPropertyChanged(nameof(SubmitButtonText));
                OnPropertyChanged(nameof(TabSignInBgColor));
                OnPropertyChanged(nameof(TabSignInTextColor));
                OnPropertyChanged(nameof(TabRegisterBgColor));
                OnPropertyChanged(nameof(TabRegisterTextColor));
                ClearError();
            }
        }
    }

    public bool IsSignInMode => !IsRegisterMode;

    public string SubmitButtonText => IsRegisterMode ? "Create Account" : "Sign In to Dashboard";

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(_errorMessage);

    public Color TabSignInBgColor => IsSignInMode ? (Color)Application.Current!.Resources["AccentPrimary"] : Colors.Transparent;
    public Color TabSignInTextColor => IsSignInMode ? Colors.White : (Color)Application.Current!.Resources["TextSub"];

    public Color TabRegisterBgColor => IsRegisterMode ? (Color)Application.Current!.Resources["AccentPrimary"] : Colors.Transparent;
    public Color TabRegisterTextColor => IsRegisterMode ? Colors.White : (Color)Application.Current!.Resources["TextSub"];

    public ICommand SwitchToLoginCommand { get; }
    public ICommand SwitchToRegisterCommand { get; }
    public ICommand SubmitCommand { get; }

    public LoginViewModel() : this(new ApiService(), new AuthManager())
    {
    }

    public LoginViewModel(IApiService apiService, IAuthManager authManager)
    {
        _apiService = apiService;
        _authManager = authManager;
        Title = "Sign In / Register";

        SwitchToLoginCommand = new Command(() => IsRegisterMode = false);
        SwitchToRegisterCommand = new Command(() => IsRegisterMode = true);
        SubmitCommand = new Command(async () => await ExecuteSubmitAsync(), () => IsNotBusy);
    }

    private async Task ExecuteSubmitAsync()
    {
        if (IsBusy) return;

        var email = Email.Trim();
        var password = Password;
        var fullName = FullName.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ErrorMessage = "Please enter both email and password.";
            return;
        }

        if (IsRegisterMode && string.IsNullOrWhiteSpace(fullName))
        {
            ErrorMessage = "Please enter your full name.";
            return;
        }

        IsBusy = true;
        ClearError();
        ((Command)SubmitCommand).ChangeCanExecute();

        try
        {
            if (IsRegisterMode)
            {
                var req = new RegisterRequest
                {
                    FullName = fullName,
                    Email = email,
                    Password = password
                };

                NavigateToApp();
                //var res = await _apiService.RegisterAsync(req);
                //if (res != null && res.Success && res.Data != null)
                //{
                //    _authManager.SetUser(res.Data);
                //    NavigateToApp();
                //}
                //else
                //{
                //    ErrorMessage = res?.Message ?? "Registration failed. Please try again.";
                //}
            }
            else
            {

                NavigateToApp();
                //var req = new LoginRequest
                //{
                //    Email = email,
                //    Password = password
                //};

                //var res = await _apiService.LoginAsync(req);
                //if (res != null && res.Success && res.Data != null)
                //{
                //    _authManager.SetUser(res.Data);
                //    NavigateToApp();
                //}
                //else
                //{
                //    ErrorMessage = res?.Message ?? "Invalid email or password.";
                //}
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Connection error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            ((Command)SubmitCommand).ChangeCanExecute();
        }
    }

    private void ClearError()
    {
        ErrorMessage = string.Empty;
    }

    private void NavigateToApp()
    {
        Application.Current!.MainPage = new AppShell();
    }
}
