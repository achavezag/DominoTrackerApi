using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DominoTracker.Application.Constants;
using DominoTracker.Application.Exceptions;
using DominoTracker.Application.DTOs;
using MatchType = DominoTracker.Domain.Entities.MatchType;

namespace DominoTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarsController : ControllerBase
{
    private readonly IWarService _warService;

    public WarsController(IWarService warService)
    {
        _warService = warService;
    }

    // GET: api/wars
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? seasonIds)
    {
        var wars = await _warService.GetAllAsync(SeasonQuery.Parse(seasonIds));
        return Ok(wars);
    }

    // GET: api/wars/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var war = await _warService.GetByIdAsync(id);

        if (war == null)
            return NotFound();

        return Ok(war);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] Guid teamAId, [FromQuery] Guid teamBId, [FromQuery] MatchType type, [FromQuery] DateTime? date)
    {
        var result = await _warService.GetMatchHistoryAsync(teamAId, teamBId, type, date);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.AdminRoles)]
    public async Task<IActionResult> Create([FromBody] CreateWarRequest request)
    {
        try
        {
            await _warService.CreateWarAsync(request.MatchIds);
            return Ok();
        }
        catch (InvalidWarMatchesException ex)
        {
            return BadRequest(new { code = "INVALID_MATCHES", message = ex.Message });
        }
        catch (NoMatchesForWarException ex)
        {
            return BadRequest(new { code = "NO_MATCHES", message = ex.Message });
        }
        catch (DuplicateWarException ex)
        {
            return Conflict(new { code = "DUPLICATE_WAR", message = ex.Message });
        }
    }
}

