namespace NotesApp.API.Models;

/// <summary>
/// Represents a note row in the database.
/// UserId links every note to its owner —
/// this is how we enforce "users can only access their own notes."
/// </summary>
public class Note
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}