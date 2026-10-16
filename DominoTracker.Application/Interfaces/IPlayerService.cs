using DominoTracker.Application.DTOs;

namespace DominoTracker.Application.Interfaces;

public interface IPlayerService
{
    Task<List<PlayerStatsResponse>> GetAllStatsAsync(List<int>? seasonIds = null);
    Task<HeadToHeadResponse> GetHeadToHeadAsync(Guid player1Id, Guid player2Id, List<int>? seasonIds = null);
}