using DominoTracker.Application.Interfaces;
using DominoTracker.Domain.Entities;
using DominoTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DominoTracker.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly AppDbContext _context;

    public PlayerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Player?> GetByIdAsync(Guid id)
    {
        return await _context.Players.FindAsync(id);
    }

    public async Task<List<Player>> GetAllAsync()
    {
        return await _context.Players.ToListAsync();
    }
}