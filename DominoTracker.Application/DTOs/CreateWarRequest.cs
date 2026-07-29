namespace DominoTracker.Application.DTOs;

public class CreateWarRequest
{
    public List<Guid> MatchIds { get; set; } = new();
}