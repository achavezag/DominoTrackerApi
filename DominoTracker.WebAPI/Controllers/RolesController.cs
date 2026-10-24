using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DominoTracker.Application.DTOs;
using DominoTracker.Application.Interfaces;

namespace DominoTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IUserService _userService;

    public RolesController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoleDto>>> GetAll()
    {
        var roles = await _userService.GetAllRolesAsync();
        return Ok(roles);
    }
}
