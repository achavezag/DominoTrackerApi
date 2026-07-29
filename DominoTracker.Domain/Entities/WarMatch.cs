namespace DominoTracker.Domain.Entities;

public class WarMatch
{
    public Guid Id { get; set; }

    public Guid WarId { get; set; }
    public War War { get; set; } = null!;

    public Guid MatchId { get; set; }
    public Match Match { get; set; } = null!;
    public int Order { get; set; }
}