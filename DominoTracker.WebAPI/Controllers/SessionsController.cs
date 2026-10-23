using Microsoft.AspNetCore.Mvc;
using DominoTracker.Application.DTOs;
using DominoTracker.Application.Interfaces;

namespace DominoTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionsController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SessionDto>>> GetAll()
    {
        var sessions = await _sessionService.GetAllAsync();
        return Ok(sessions);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSessionRequest request)
    {
        try
        {
            var session = await _sessionService.CreateAsync(request);
            return Ok(session);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
