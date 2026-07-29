namespace DominoTracker.Application.DTOs;

public class WarDetailResponse
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string TeamAName { get; set; } = string.Empty;
    public string TeamBName { get; set; } = string.Empty;
    public Guid? WinningTeamId { get; set; }
    public List<MatchListItemResponse> Matches { get; set; } = new();
}