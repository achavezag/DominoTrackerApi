namespace DominoTracker.Domain.Entities;

public class Match
{
    public Guid Id { get; set; }

    public Guid TeamAId { get; set; }
    public Guid TeamBId { get; set; }

    public Team TeamA { get; set; } = null!;
    public Team TeamB { get; set; } = null!;

    public int TeamAScore { get; set; }
    public int TeamBScore { get; set; }

    public Guid? WinningTeamId { get; set; }

    public MatchType Type { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.InProgress;

    public DateTime? FinishedAt { get; set; }

    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;
}