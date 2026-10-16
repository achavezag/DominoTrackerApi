using DominoTracker.Application.DTOs;
using MatchType = DominoTracker.Domain.Entities.MatchType;

public interface IWarService
{
    Task<List<WarListItemResponse>> GetAllAsync(List<int>? seasonIds = null);
    Task<WarDetailResponse?> GetByIdAsync(Guid id);
    Task<List<MatchHistoryItemResponse>> GetMatchHistoryAsync(Guid teamAId, Guid teamBId, MatchType type);
    Task CreateWarAsync(List<Guid> matchIds);
}