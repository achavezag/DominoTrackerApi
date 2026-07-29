using DominoTracker.Domain.Entities;

namespace DominoTracker.Application.Interfaces;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(Guid id);
    Task<Match?> GetByIdWithDetailsAsync(Guid id);
    Task SaveChangesAsync();
    Task<List<Match>> GetAllWithTeamsAsync();
    Task AddAsync(Match match);
}