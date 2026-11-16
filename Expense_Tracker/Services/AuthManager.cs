using Expense_Tracker.Models;

namespace Expense_Tracker.Services;

public interface IAuthManager
{
    UserDto? CurrentUser { get; }
    bool IsAuthenticated { get; }
    void SetUser(UserDto user);
    void Logout();
}

public class AuthManager : IAuthManager
{
    private const string KeyUserEmail = "Auth_UserEmail";
    private const string KeyUserName = "Auth_UserName";
    private const string KeyUserId = "Auth_UserId";

    public UserDto? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser != null;

    public AuthManager()
    {
        var email = Preferences.Default.Get(KeyUserEmail, string.Empty);
        var name = Preferences.Default.Get(KeyUserName, string.Empty);
        var id = Preferences.Default.Get(KeyUserId, 0);

        if (!string.IsNullOrEmpty(email))
        {
            CurrentUser = new UserDto
            {
                Id = id,
                Email = email,
                FullName = name
            };
        }
    }

    public void SetUser(UserDto user)
    {
        CurrentUser = user;
        Preferences.Default.Set(KeyUserEmail, user.Email);
        Preferences.Default.Set(KeyUserName, user.FullName);
        Preferences.Default.Set(KeyUserId, user.Id);
    }

    public void Logout()
    {
        CurrentUser = null;
        Preferences.Default.Remove(KeyUserEmail);
        Preferences.Default.Remove(KeyUserName);
        Preferences.Default.Remove(KeyUserId);
    }
}
