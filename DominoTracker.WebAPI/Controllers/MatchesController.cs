using Microsoft.AspNetCore.Mvc;
using DominoTracker.Application.Services;
using DominoTracker.Infrastructure.Persistence;
using DominoTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DominoTracker.Application.DTOs;

namespace DominoTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly MatchService _matchService;
    private readonly AppDbContext _context;

    public MatchesController(AppDbContext context, MatchService matchService)
    {
        _context = context;
        _matchService = matchService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMatch(CreateMatchRequest request)
    {
        try
        {
            var match = await _matchService.CreateMatchAsync(request);
            return Ok(match);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{matchId}")]
    public async Task<IActionResult> GetById(Guid matchId)
    {
        var match = await _matchService.GetMatchDetailAsync(matchId);

        if (match == null)
            return NotFound();

        return Ok(match);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? type,
        [FromQuery] Guid? teamAId,
        [FromQuery] Guid? teamBId,
        [FromQuery] string? seasonIds
    )
    {
        var matches = await _matchService.GetFilteredAsync(
            fromDate, toDate, type, teamAId, teamBId, SeasonQuery.Parse(seasonIds)
        );

        return Ok(matches);
    }

}