using System.ComponentModel.DataAnnotations;

namespace NotesApp.API.DTOs.Auth;

/// <summary>
/// The data the client sends when logging in.
/// We use email + password as credentials.
/// </summary>
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}