using DominoTracker.Domain.Entities;

namespace DominoTracker.Application.Interfaces;

public interface IPlayerRepository
{
    Task<Player?> GetByIdAsync(Guid id);
    Task<List<Player>> GetAllAsync();
}