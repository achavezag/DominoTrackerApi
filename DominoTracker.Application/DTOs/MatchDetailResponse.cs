namespace DominoTracker.Application.DTOs;

public class MatchDetailResponse
{
    public Guid Id { get; set; }

    public TeamDto TeamA { get; set; } = null!;
    public TeamDto TeamB { get; set; } = null!;

    public int TeamAScore { get; set; }
    public int TeamBScore { get; set; }

    public Guid? WinningTeamId { get; set; }
    public string Type { get; set; } = null!;
    public string Status { get; set; } = null!;

}