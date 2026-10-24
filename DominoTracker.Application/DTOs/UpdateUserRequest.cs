namespace DominoTracker.Application.DTOs;

public class UpdateUserRequest
{
    public string Username { get; set; } = null!;
    public string? Password { get; set; }
    public int RoleId { get; set; }
    public int? SessionId { get; set; }
}
