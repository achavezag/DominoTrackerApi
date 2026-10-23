namespace DominoTracker.Application.DTOs;

public class TeamListItemResponse
{
    public Guid Id { get; set; }
    public PlayerDto Player1 { get; set; } = null!;
    public PlayerDto Player2 { get; set; } = null!;
    public SeasonPlayedDto[] SeasonsPlayed { get; set; } = [];
}
