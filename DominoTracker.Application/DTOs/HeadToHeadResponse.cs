namespace DominoTracker.Application.DTOs;

public class HeadToHeadResponse
{
    public Guid Player1Id { get; set; }
    public Guid Player2Id { get; set; }

    public int MatchesPlayed { get; set; }

    // MATCHES TOTALES
    public int Player1Wins { get; set; }
    public int Player2Wins { get; set; }

    // MATCHES QUE PERTENECEN A WARS
    public int Player1WarMatchWins { get; set; }
    public int Player2WarMatchWins { get; set; }

    // WARS GANADAS
    public int Player1WarWins { get; set; }
    public int Player2WarWins { get; set; }

}

