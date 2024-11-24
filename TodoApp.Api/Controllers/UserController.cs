using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Shared.Interfaces;
using TodoApp.Shared.Models;
using TodoApp.Shared.Models.Requests;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
            return Unauthorized();

        var profile = await _userService.GetUserProfileAsync(id);
        if (profile == null)
            return NotFound();

        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
            return Unauthorized();

        var success = await _userService.UpdateUserProfileAsync(id, request);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
