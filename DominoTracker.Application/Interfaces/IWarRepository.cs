using DominoTracker.Domain.Entities;
using MatchType = DominoTracker.Domain.Entities.MatchType;

namespace DominoTracker.Application.Interfaces;

public interface IWarRepository
{
    Task<List<War>> GetAllAsync();
    Task<War?> GetByIdAsync(Guid id);
    Task<bool> ExistsByDateAsync(DateTime date);
    Task<List<Match>> GetMatchesForTodayAsync(DateTime date);
    Task<List<Match>> GetMatchHistoryAsync(Guid teamAId, Guid teamBId, MatchType type, DateTime date);
    Task AddAsync(War war);
    Task AddWarMatchesAsync(List<WarMatch> warMatches);
    Task SaveChangesAsync();
    Task<List<Match>> GetMatchesByIdsAsync(List<Guid> ids);
    Task<List<List<Guid>>> GetWarMatchIdsAsync();
    Task<List<War>> GetAllWithMatchesAsync();
    Task<List<War>> GetAllTeamWithMatchesAsync();
}