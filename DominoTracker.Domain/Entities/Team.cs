namespace DominoTracker.Domain.Entities;

public class Team
{
    public Guid Id { get; set; }

    public Guid Player1Id { get; set; }
    public Guid Player2Id { get; set; }

    public Player Player1 { get; set; } = null!;
    public Player Player2 { get; set; } = null!;

    public static Team Create(Guid player1Id, Guid player2Id)
    {
        if (player1Id.CompareTo(player2Id) > 0)
        {
            (player1Id, player2Id) = (player2Id, player1Id);
        }

        return new Team
        {
            Id = Guid.NewGuid(),
            Player1Id = player1Id,
            Player2Id = player2Id
        };
    }
}