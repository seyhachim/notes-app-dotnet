using Microsoft.AspNetCore.Mvc;
using NotesApp.API.DTOs.Auth;
using NotesApp.API.Services;

namespace NotesApp.API.Controllers;

/// <summary>
/// Handles HTTP requests for authentication.
/// Intentionally thin — all logic is delegated to AuthService.
/// This controller's only job is: parse the request, call the service, return the response.
/// </summary>
[ApiController]
[Route("api/[controller]")]  // resolves to /api/auth
public class AuthController(AuthService authService) : ControllerBase
{
    /// <summary>
    /// POST /api/auth/register
    /// Creates a new user account.
    /// Returns 201 Created with the new user Id on success.
    /// Returns 400 Bad Request if validation fails.
    /// Returns 500 (for now) if email/username is taken — Step 7 will fix this.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // ModelState checks the DataAnnotations on RegisterRequest.
        // [ApiController] actually does this automatically, but keeping it
        // explicit makes the validation intent obvious to a code reviewer.
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = await authService.RegisterAsync(request);

        // 201 Created is more semantically correct than 200 OK for resource creation.
        // The second argument provides the location of the new resource.
        return CreatedAtAction(nameof(Register), new { id = userId });
    }


    /// <summary>
    /// POST /api/auth/login
    /// Returns a JWT token on success.
    /// Returns 401 Unauthorized if credentials are wrong.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await authService.LoginAsync(request);
        return Ok(response);
    }
}