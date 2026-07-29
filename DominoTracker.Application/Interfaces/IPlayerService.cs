using DominoTracker.Application.DTOs;

namespace DominoTracker.Application.Interfaces;

public interface IPlayerService
{
    Task<List<PlayerStatsResponse>> GetAllStatsAsync();
    Task<HeadToHeadResponse> GetHeadToHeadAsync(Guid player1Id, Guid player2Id);
}