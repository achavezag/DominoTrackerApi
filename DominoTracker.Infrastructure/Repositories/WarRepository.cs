using Microsoft.EntityFrameworkCore;
using DominoTracker.Domain.Entities;
using DominoTracker.Application.Interfaces;
using DominoTracker.Infrastructure.Persistence;
using MatchType = DominoTracker.Domain.Entities.MatchType;

public class WarRepository : IWarRepository
{
    private readonly AppDbContext _context;

    public WarRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<War>> GetAllAsync()
    {
        return await _context.Wars
            .AsNoTracking()
            .Include(w => w.TeamA)
                .ThenInclude(t => t.Player1)
            .Include(w => w.TeamA)
                .ThenInclude(t => t.Player2)
            .Include(w => w.TeamB)
                .ThenInclude(t => t.Player1)
            .Include(w => w.TeamB)
                .ThenInclude(t => t.Player2)
            .OrderByDescending(w => w.Date)
            .ToListAsync();
    }

    public async Task<War?> GetByIdAsync(Guid id)
    {
        return await _context.Wars
            .Include(w => w.WarMatches)
                .ThenInclude(wm => wm.Match)
                    .ThenInclude(m => m.TeamA)
                        .ThenInclude(t => t.Player1)
            .Include(w => w.WarMatches)
                .ThenInclude(wm => wm.Match)
                    .ThenInclude(m => m.TeamA)
                        .ThenInclude(t => t.Player2)
            .Include(w => w.WarMatches)
                .ThenInclude(wm => wm.Match)
                    .ThenInclude(m => m.TeamB)
                        .ThenInclude(t => t.Player1)
            .Include(w => w.WarMatches)
                .ThenInclude(wm => wm.Match)
                    .ThenInclude(m => m.TeamB)
                        .ThenInclude(t => t.Player2)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<bool> ExistsByDateAsync(DateTime date)
    {
        return await _context.Wars.AnyAsync(w => w.Date == date);
    }

    public async Task<List<Match>> GetMatchesForTodayAsync(DateTime date)
    {
        return await _context.Matches
            .Where(m =>
                m.Type.ToString() == "Fixed" &&
                m.FinishedAt.HasValue &&
                m.FinishedAt.Value.Date == date
            )
            .OrderBy(m => m.FinishedAt)
            .ToListAsync();
    }

    public async Task<List<Match>> GetMatchHistoryAsync(Guid teamAId, Guid teamBId, MatchType type, DateTime date)
    {
        return await _context.Matches
            .Where(m =>
                m.Type == type &&
                m.FinishedAt.HasValue &&
                m.FinishedAt.Value.Date == date &&
                (
                    (m.TeamAId == teamAId && m.TeamBId == teamBId) ||
                    (m.TeamAId == teamBId && m.TeamBId == teamAId)
                )
            )
            .OrderBy(m => m.FinishedAt)
            .ToListAsync();
    }

    public async Task AddAsync(War war)
    {
        await _context.Wars.AddAsync(war);
    }

    public async Task AddWarMatchesAsync(List<WarMatch> warMatches)
    {
        await _context.WarMatches.AddRangeAsync(warMatches);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Match>> GetMatchesByIdsAsync(List<Guid> ids)
    {
        return await _context.Matches
            .Where(m => ids.Contains(m.Id))
            .ToListAsync();
    }

    public async Task<List<List<Guid>>> GetWarMatchIdsAsync()
    {
        return await _context.WarMatches
       .GroupBy(wm => wm.WarId)
       .Select(g => g
           .Select(x => x.MatchId)
           .OrderBy(id => id)
           .ToList()
       )
       .ToListAsync();
    }

    public async Task<List<War>> GetAllWithMatchesAsync()
    {
        return await _context.Wars
            .Include(w => w.WarMatches)
                .ThenInclude(wm => wm.Match)
                    .ThenInclude(m => m.TeamA)
                        .ThenInclude(t => t.Player1)

            .Include(w => w.WarMatches)
                .ThenInclude(wm => wm.Match)
                    .ThenInclude(m => m.TeamA)
                        .ThenInclude(t => t.Player2)

            .Include(w => w.WarMatches)
                .ThenInclude(wm => wm.Match)
                    .ThenInclude(m => m.TeamB)
                        .ThenInclude(t => t.Player1)

            .Include(w => w.WarMatches)
                .ThenInclude(wm => wm.Match)
                    .ThenInclude(m => m.TeamB)
                        .ThenInclude(t => t.Player2)

            .ToListAsync();
    }

    public async Task<List<War>> GetAllTeamWithMatchesAsync()
    {
        return await _context.Wars
            .Include(w => w.WarMatches)
                .ThenInclude(wm => wm.Match)
            .ToListAsync();
    }
}