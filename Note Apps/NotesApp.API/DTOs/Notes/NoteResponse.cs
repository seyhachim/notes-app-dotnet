namespace NotesApp.API.DTOs.Notes;

/// <summary>
/// What we send back to the client.
/// We never send UserId back — the client doesn't need it
/// and it's cleaner not to expose internal IDs unnecessarily.
/// </summary>
public class NoteResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}