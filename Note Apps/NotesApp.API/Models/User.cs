namespace NotesApp.API.Models;

/// <summary>
/// Represents a user row in the database.
/// This is a plain data class — no logic, no validation.
/// Dapper maps column names to these properties automatically.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}