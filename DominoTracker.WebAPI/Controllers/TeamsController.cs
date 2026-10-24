using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DominoTracker.Application.Constants;
using DominoTracker.Infrastructure.Persistence;
using DominoTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DominoTracker.Application.DTOs;
using DominoTracker.Application.Interfaces;
using DominoTracker.Application.Services;

namespace DominoTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TeamStatsService _teamStatsService;
    private readonly ICurrentSessionAccessor _currentSessionAccessor;

    public TeamsController(AppDbContext context, TeamStatsService teamStatsService, ICurrentSessionAccessor currentSessionAccessor)
    {
        _context = context;
        _teamStatsService = teamStatsService;
        _currentSessionAccessor = currentSessionAccessor;
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.AdminRoles)]
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
            Player2Id = p2,
            SessionId = _currentSessionAccessor.SessionId
        };

        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        return Ok(team);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var teams = await _context.Teams
            .Include(t => t.Player1)
            .Include(t => t.Player2)
            .ToListAsync();

        var teamIds = teams.Select(t => t.Id).ToList();

        var matchSeasons = await _context.Matches
            .Where(m => teamIds.Contains(m.TeamAId) || teamIds.Contains(m.TeamBId))
            .Select(m => new { m.TeamAId, m.TeamBId, m.SeasonId })
            .ToListAsync();

        var seasonById = await _context.Seasons
            .Select(s => new { s.Id, s.Alias })
            .ToDictionaryAsync(s => s.Id);

        var result = teams.Select(t => new TeamListItemResponse
        {
            Id = t.Id,
            Player1 = new PlayerDto
            {
                Id = t.Player1.Id,
                Name = t.Player1.Name,
                Nickname = t.Player1.Nickname,
                Image = t.Player1.Image
            },
            Player2 = new PlayerDto
            {
                Id = t.Player2.Id,
                Name = t.Player2.Name,
                Nickname = t.Player2.Nickname,
                Image = t.Player2.Image
            },
            SeasonsPlayed = matchSeasons
                .Where(m => m.TeamAId == t.Id || m.TeamBId == t.Id)
                .Select(m => m.SeasonId)
                .Distinct()
                .OrderBy(id => id)
                .Select(id => new SeasonPlayedDto { Id = id, Alias = seasonById[id].Alias })
                .ToArray()
        });

        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<List<TeamStatsResponse>>> GetStats([FromQuery] string? seasonIds)
    {
        var stats = await _teamStatsService.GetAllStatsAsync(SeasonQuery.Parse(seasonIds));
        return Ok(stats);
    }
}