namespace DominoTracker.Application.DTOs;

public class TeamDto
{
    public Guid Id { get; set; }
    public PlayerDto Player1 { get; set; } = null!;
    public PlayerDto Player2 { get; set; } = null!;
}