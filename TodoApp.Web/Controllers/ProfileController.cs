using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApp.Web.Services;
using TodoApp.Web.Models;

namespace TodoApp.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IUserWebService _userService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(
        IUserWebService userService,
        ILogger<ProfileController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var userProfile = await _userService.GetUserProfileAsync(Guid.Parse(userId));
            if (userProfile == null)
            {
                return NotFound();
            }

            return View(userProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user profile");
            TempData["ErrorMessage"] = "Error al cargar el perfil. Por favor, inténtalo de nuevo.";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UserProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (userId != model.Id.ToString())
            {
                return Forbid();
            }

            var success = await _userService.UpdateUserProfileAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "No se pudo actualizar el perfil. Por favor, inténtalo de nuevo.");
                return View("Index", model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            ModelState.AddModelError("", "Error al actualizar el perfil. Por favor, inténtalo de nuevo.");
            return View("Index", model);
        }
    }
}
