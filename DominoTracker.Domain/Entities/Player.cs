namespace DominoTracker.Domain.Entities;

public class Player
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Nickname { get; set; } = null!;
}