namespace DominoTracker.Application.DTOs;

public class WarListItemResponse
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string TeamAName { get; set; } = string.Empty;
    public string TeamBName { get; set; } = string.Empty;
}