using DominoTracker.Application.Interfaces;
using DominoTracker.Domain.Entities;
using DominoTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DominoTracker.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly AppDbContext _context;

    public TeamRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByPlayersAsync(Guid player1Id, Guid player2Id)
    {
        return await _context.Teams
            .FirstOrDefaultAsync(t =>
                t.Player1Id == player1Id &&
                t.Player2Id == player2Id);
    }

    public async Task AddAsync(Team team)
    {
        await _context.Teams.AddAsync(team);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await _context.Teams
            .Include(t => t.Player1)
            .Include(t => t.Player2)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Team>> GetAllAsync()
    {
        return await _context.Teams
            .Include(t => t.Player1)
            .Include(t => t.Player2)
            .ToListAsync();
    }
}