using DominoTracker.Application.Interfaces;
using DominoTracker.Domain.Entities;
using DominoTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MatchType = DominoTracker.Domain.Entities.MatchType;

namespace DominoTracker.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly AppDbContext _context;

    public MatchRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Match?> GetByIdAsync(Guid id)
    {
        return await _context.Matches.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Match?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Matches
            .Include(m => m.TeamA).ThenInclude(t => t.Player1)
            .Include(m => m.TeamA).ThenInclude(t => t.Player2)
            .Include(m => m.TeamB).ThenInclude(t => t.Player1)
            .Include(m => m.TeamB).ThenInclude(t => t.Player2)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Match>> GetByTypeAsync(MatchType type)
    {
        return await _context.Matches
            .Where(m => m.Type == type)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Match>> GetAllWithTeamsAsync()
    {
        return await _context.Matches
            .Include(m => m.TeamA).ThenInclude(t => t.Player1)
            .Include(m => m.TeamA).ThenInclude(t => t.Player2)
            .Include(m => m.TeamB).ThenInclude(t => t.Player1)
            .Include(m => m.TeamB).ThenInclude(t => t.Player2)
            .ToListAsync();
    }

    public async Task AddAsync(Match match)
    {
        await _context.Matches.AddAsync(match);
    }
}