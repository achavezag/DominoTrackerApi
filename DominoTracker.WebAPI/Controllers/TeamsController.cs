using Microsoft.AspNetCore.Mvc;
using DominoTracker.Infrastructure.Persistence;
using DominoTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DominoTracker.Application.DTOs;
using DominoTracker.Application.Services;

namespace DominoTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TeamStatsService _teamStatsService;

    public TeamsController(AppDbContext context, TeamStatsService teamStatsService)
    {
        _context = context;
        _teamStatsService = teamStatsService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeamRequest request)
    {
        if (request.Player1Id == request.Player2Id)
            return BadRequest("Players must be different");

        var p1 = request.Player1Id;
        var p2 = request.Player2Id;

        // Ordenar (clave 🔥)
        if (p1.CompareTo(p2) > 0)
        {
            (p1, p2) = (p2, p1);
        }

        // Ver si ya existe
        var existing = await _context.Teams
            .FirstOrDefaultAsync(t => t.Player1Id == p1 && t.Player2Id == p2);

        if (existing != null)
            return Ok(existing);

        var team = new Team
        {
            Id = Guid.NewGuid(),
            Player1Id = p1,
            Player2Id = p2
        };

        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        return Ok(team);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.Teams
            .Include(t => t.Player1)
            .Include(t => t.Player2)
            .ToList());
    }

    [HttpGet("stats")]
    public async Task<ActionResult<List<TeamStatsResponse>>> GetStats()
    {
        var stats = await _teamStatsService.GetAllStatsAsync();
        return Ok(stats);
    }
}