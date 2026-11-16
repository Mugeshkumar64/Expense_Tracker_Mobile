using System.Text.Json.Serialization;

namespace Expense_Tracker.Models;

public class RegisterRequest
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

public class UserDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    public string Initials
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FullName)) return "ME";
            var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1
                ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                : FullName[..Math.Min(2, FullName.Length)].ToUpper();
        }
    }
}
