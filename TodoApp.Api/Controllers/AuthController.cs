using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Services;
using TodoApp.Shared.Models;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> Login(UserLogin login)
    {
        var response = await _authService.LoginAsync(login);
        if (response == null)
        {
            return Unauthorized("Usuario o contraseña incorrectos");
        }

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(UserRegister register)
    {
        var response = await _authService.RegisterAsync(register);
        if (response == null)
        {
            return BadRequest("No se pudo registrar el usuario. El nombre de usuario ya existe o las contraseñas no coinciden.");
        }

        return Ok(response);
    }
}
