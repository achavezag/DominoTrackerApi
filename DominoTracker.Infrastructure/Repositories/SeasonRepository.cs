using DominoTracker.Application.Interfaces;
using DominoTracker.Domain.Entities;
using DominoTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DominoTracker.Infrastructure.Repositories;

public class SeasonRepository : ISeasonRepository
{
    private readonly AppDbContext _context;

    public SeasonRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Season>> GetAllAsync()
    {
        return await _context.Seasons
            .OrderBy(s => s.StartDate)
            .ToListAsync();
    }

    public async Task<Season?> GetByIdAsync(int id)
    {
        return await _context.Seasons.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Season?> GetByDateAsync(DateTime date)
    {
        var day = date.Date;

        return await _context.Seasons
            .FirstOrDefaultAsync(s => day >= s.StartDate.Date && day <= s.EndDate.Date);
    }

    public async Task<bool> HasDependenciesAsync(int id)
    {
        return await _context.Matches.AnyAsync(m => m.SeasonId == id)
            || await _context.Wars.AnyAsync(w => w.SeasonId == id);
    }

    public async Task<bool> HasMatchesAsync(int id)
    {
        return await _context.Matches.AnyAsync(m => m.SeasonId == id);
    }

    public async Task<bool> HasMatchesAfterDateAsync(int id, DateTime date)
    {
        var day = date.Date;

        return await _context.Matches.AnyAsync(m =>
            m.SeasonId == id &&
            m.FinishedAt.HasValue &&
            m.FinishedAt.Value.Date > day);
    }

    public async Task AddAsync(Season season)
    {
        await _context.Seasons.AddAsync(season);
    }

    public async Task RemoveAsync(Season season)
    {
        _context.Seasons.Remove(season);
        await Task.CompletedTask;
    }

    public async Task ReseedIdentityAsync()
    {
        var maxId = await _context.Seasons.AnyAsync()
            ? await _context.Seasons.MaxAsync(s => s.Id)
            : 0;

        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"DBCC CHECKIDENT ('Seasons', RESEED, {maxId})");
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
