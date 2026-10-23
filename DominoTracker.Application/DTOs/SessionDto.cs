namespace DominoTracker.Application.DTOs;

public class SessionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
