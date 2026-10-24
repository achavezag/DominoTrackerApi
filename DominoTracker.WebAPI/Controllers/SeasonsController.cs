using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DominoTracker.Application.Constants;
using DominoTracker.Application.DTOs;
using DominoTracker.Application.Exceptions;
using DominoTracker.Application.Interfaces;

namespace DominoTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeasonsController : ControllerBase
{
    private readonly ISeasonService _seasonService;

    public SeasonsController(ISeasonService seasonService)
    {
        _seasonService = seasonService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SeasonDto>>> GetAll()
    {
        var seasons = await _seasonService.GetAllAsync();
        return Ok(seasons);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SeasonDto>> GetById(int id)
    {
        try
        {
            var season = await _seasonService.GetByIdAsync(id);
            return Ok(season);
        }
        catch (SeasonNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("by-date")]
    public async Task<ActionResult<SeasonDto>> GetByDate([FromQuery] DateTime? date)
    {
        var targetDate = date?.Date ?? DateTime.Today;

        try
        {
            var season = await _seasonService.GetByDateAsync(targetDate);
            return Ok(season);
        }
        catch (SeasonNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.AdminRoles)]
    public async Task<IActionResult> Create(CreateSeasonRequest request)
    {
        try
        {
            var season = await _seasonService.CreateAsync(request);
            return Ok(season);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = RoleNames.AdminRoles)]
    public async Task<IActionResult> Update(int id, UpdateSeasonRequest request)
    {
        try
        {
            var season = await _seasonService.UpdateAsync(id, request);
            return Ok(season);
        }
        catch (SeasonNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleNames.AdminRoles)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _seasonService.DeleteAsync(id);
            return NoContent();
        }
        catch (SeasonNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (SeasonInUseException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
