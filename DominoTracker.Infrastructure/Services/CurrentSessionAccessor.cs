using DominoTracker.Application.Interfaces;

namespace DominoTracker.Infrastructure.Services;

public class CurrentSessionAccessor : ICurrentSessionAccessor
{
    public int SessionId { get; set; } = 1;
}
