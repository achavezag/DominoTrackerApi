namespace DominoTracker.Domain.Entities;

public class War
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }

    public Guid TeamAId { get; set; }
    public Guid TeamBId { get; set; }
    public Guid? WinningTeamId { get; set; }

    public Team TeamA { get; set; } = null!;
    public Team TeamB { get; set; } = null!;

    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;

    public List<WarMatch> WarMatches { get; set; } = new();
}