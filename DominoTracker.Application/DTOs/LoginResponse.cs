namespace DominoTracker.Application.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }

    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;

    public int? SessionId { get; set; }
    public string? SessionName { get; set; }
}
