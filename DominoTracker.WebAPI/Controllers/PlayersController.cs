using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DominoTracker.Infrastructure.Persistence;
using DominoTracker.Domain.Entities;
using DominoTracker.Application.DTOs;
using DominoTracker.Application.Interfaces;

namespace DominoTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPlayerService _playerService;
    private readonly ICurrentSessionAccessor _currentSessionAccessor;

    public PlayersController(AppDbContext context, IPlayerService playerService, ICurrentSessionAccessor currentSessionAccessor)
    {
        _context = context;
        _playerService = playerService;
        _currentSessionAccessor = currentSessionAccessor;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePlayerRequest request)
    {
        var player = new Player
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Nickname = request.Nickname,
            Image = request.Image,
            SessionId = _currentSessionAccessor.SessionId
        };

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return Ok(player);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.Players.ToList());
    }

    [HttpPut("{id:guid}/image")]
    public async Task<IActionResult> UpdateImage(Guid id, UpdatePlayerImageRequest request)
    {
        var player = await _context.Players.FirstOrDefaultAsync(p => p.Id == id);

        if (player == null)
            return NotFound($"No existe un jugador con id {id}");

        player.Image = request.Image;
        await _context.SaveChangesAsync();

        return Ok(player);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<List<PlayerStatsResponse>>> GetAllStats([FromQuery] string? seasonIds)
    {
        var stats = await _playerService.GetAllStatsAsync(SeasonQuery.Parse(seasonIds));
        return Ok(stats);
    }

    [HttpGet("headtohead")]
    public async Task<ActionResult<HeadToHeadResponse>> GetHeadToHead(
        [FromQuery] Guid player1Id,
        [FromQuery] Guid player2Id,
        [FromQuery] string? seasonIds)
    {
        try
        {
            var result = await _playerService.GetHeadToHeadAsync(
                player1Id,
                player2Id,
                SeasonQuery.Parse(seasonIds)
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}