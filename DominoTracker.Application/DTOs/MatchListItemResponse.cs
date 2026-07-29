namespace DominoTracker.Application.DTOs;

public class MatchListItemResponse
{
    public Guid Id { get; set; }

    public string TeamAName { get; set; } = null!;
    public string TeamBName { get; set; } = null!;

    public int TeamAScore { get; set; }
    public int TeamBScore { get; set; }
    public Guid? WinningTeamId { get; set; }

    public string Status { get; set; } = null!;
    public string Type { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}