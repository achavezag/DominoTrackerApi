namespace DominoTracker.Application.DTOs;

public class CreateTeamRequest
{
    public Guid Player1Id { get; set; }
    public Guid Player2Id { get; set; }
}