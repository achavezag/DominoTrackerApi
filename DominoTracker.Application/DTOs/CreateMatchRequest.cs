using MatchType = DominoTracker.Domain.Entities.MatchType;

namespace DominoTracker.Application.DTOs;

public class CreateMatchRequest
{
    public Guid TeamAId { get; set; }
    public Guid TeamBId { get; set; }
    public MatchType Type { get; set; }
    public int TeamAScore { get; set; }
    public int TeamBScore { get; set; }
    public DateTime Date { get; set; }
}