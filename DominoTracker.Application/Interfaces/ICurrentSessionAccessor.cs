namespace DominoTracker.Application.Interfaces;

public interface ICurrentSessionAccessor
{
    int SessionId { get; set; }
}
