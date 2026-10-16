using Microsoft.AspNetCore.Mvc;
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

    public PlayersController(AppDbContext context, IPlayerService playerService)
    {
        _context = context;
        _playerService = playerService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePlayerRequest request)
    {
        var player = new Player
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Nickname = request.Nickname
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