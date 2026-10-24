namespace DominoTracker.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;

    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;

    public int? SessionId { get; set; }
    public string? SessionName { get; set; }
}
