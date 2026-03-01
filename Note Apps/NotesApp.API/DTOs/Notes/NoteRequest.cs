using System.ComponentModel.DataAnnotations;

namespace NotesApp.API.DTOs.Notes;

/// <summary>
/// Used for both Create and Update requests.
/// The client only sends Title and Content —
/// UserId comes from the JWT token, never from the client.
/// Letting the client send their own UserId would be a security hole.
/// </summary>
public class NoteRequest
{
    [Required]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public bool IsPinned { get; set; } = false;
}