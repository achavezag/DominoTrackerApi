using DominoTracker.Application.DTOs;
using DominoTracker.Domain.Entities;

namespace DominoTracker.Application.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByPlayersAsync(Guid player1Id, Guid player2Id);
    Task AddAsync(Team team);
    Task SaveChangesAsync();
    Task<Team?> GetByIdAsync(Guid id);
    Task<List<Team>> GetAllAsync();
}