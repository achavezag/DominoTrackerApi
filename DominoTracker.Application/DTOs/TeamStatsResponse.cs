namespace DominoTracker.Application.DTOs;

public class TeamStatsResponse
{
    public Guid TeamId { get; set; }

    public Guid Player1Id { get; set; }
    public string Player1Nickname { get; set; } = null!;

    public Guid Player2Id { get; set; }
    public string Player2Nickname { get; set; } = null!;

    public int MatchesPlayed { get; set; }
    public int[] SeasonsPlayed { get; set; } = [];
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