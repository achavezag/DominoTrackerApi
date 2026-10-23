using DominoTracker.Domain.Entities;

namespace DominoTracker.Application.Interfaces;

public interface ISeasonRepository
{
    Task<List<Season>> GetAllAsync();
    Task<Season?> GetByIdAsync(int id);
    Task<Season?> GetByDateAsync(DateTime date);
    Task<bool> HasDependenciesAsync(int id);
    Task<bool> HasMatchesAsync(int id);
    Task<bool> HasMatchesAfterDateAsync(int id, DateTime date);
    Task AddAsync(Season season);
    Task RemoveAsync(Season season);
    Task ReseedIdentityAsync();
    Task<int> GetNextAliasAsync();
    Task RenumberAliasesAsync();
    Task SaveChangesAsync();
}
