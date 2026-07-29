namespace DominoTracker.Application.DTOs;

public class MatchHistoryItemResponse
{
    public Guid Id { get; set; }
    public Guid TeamAId { get; set; }
    public Guid TeamBId { get; set; }
    public int TeamAScore { get; set; }
    public int TeamBScore { get; set; }
    public DateTime? FinishedAt { get; set; }
}