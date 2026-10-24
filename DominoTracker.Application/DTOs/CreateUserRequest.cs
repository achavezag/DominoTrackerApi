namespace DominoTracker.Application.DTOs;

public class CreateUserRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int RoleId { get; set; }
    public int? SessionId { get; set; }
}
