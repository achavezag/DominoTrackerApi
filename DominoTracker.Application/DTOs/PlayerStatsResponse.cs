namespace DominoTracker.Application.DTOs;

public class PlayerStatsResponse
{
    public Guid PlayerId { get; set; }
    public string Name { get; set; } = null!;
    public string Nickname { get; set; } = null!;

    public int MatchesPlayed { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public double WinRate { get; set; }

    public int FixedMatches { get; set; }
    public int FixedWins { get; set; }
    public int FixedLosses { get; set; }
    public double FixedMatchesWinRate { get; set; }

    public int RotatingMatches { get; set; }
    public int RotatingWins { get; set; }
    public int RotatingLosses { get; set; }
    public double RotatingMatchesWinRate { get; set; }

    public int WarsPlayed { get; set; }
    public int WarWins { get; set; }
    public int WarLosses { get; set; }
    public double WarWinRate { get; set; }
}