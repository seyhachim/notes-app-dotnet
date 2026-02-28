namespace NotesApp.API.DTOs.Auth;

/// <summary>
/// What we send back to the client after successful login or register.
/// The client stores this token and sends it with every future request.
/// </summary>
public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}