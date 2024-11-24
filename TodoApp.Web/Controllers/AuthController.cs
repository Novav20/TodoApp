using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApp.Shared.Interfaces;
using TodoApp.Shared.Models.Requests;
using TodoApp.Shared.Models.Responses;
using TodoApp.Web.Models;
using TodoApp.Web.Services;

namespace TodoApp.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthWebService _authService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthWebService authService, 
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Login attempt failed due to invalid model state");
            return View(model);
        }

        try 
        {
            var loginRequest = new UserLogin
            {
                Username = model.Username ?? string.Empty,
                Password = model.Password ?? string.Empty
            };

            var response = await _authService.LoginAsync(loginRequest);
            if (response == null || string.IsNullOrEmpty(response.Token))
            {
                _logger.LogWarning("Login failed for user {Username}", model.Username);
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            _logger.LogInformation("User {Username} logged in successfully", response.Username);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, response.Username ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, response.Id.ToString()),
                new Claim("Token", response.Token)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                RedirectUri = returnUrl
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", model.Username);
            ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
            return View(model);
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Registration attempt failed due to invalid model state");
            return View(model);
        }

        try
        {
            var registerRequest = new UserRegister
            {
                Username = model.Username ?? string.Empty,
                Email = model.Email ?? string.Empty,
                Password = model.Password ?? string.Empty,
                ConfirmPassword = model.ConfirmPassword ?? string.Empty
            };

            var response = await _authService.RegisterAsync(registerRequest);
            if (response == null || string.IsNullOrEmpty(response.Token))
            {
                _logger.LogWarning("Registration failed for user {Username}", model.Username);
                ModelState.AddModelError(string.Empty, "Registration failed. Username or email might already be in use.");
                return View(model);
            }

            _logger.LogInformation("User {Username} registered successfully", response.Username);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, response.Username ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, response.Id.ToString()),
                new Claim("Token", response.Token)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                RedirectUri = returnUrl
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user {Username}", model.Username);
            ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var username = User.Identity?.Name;
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("User {Username} logged out", username);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
