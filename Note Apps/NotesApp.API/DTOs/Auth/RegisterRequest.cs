using System.ComponentModel.DataAnnotations;

namespace NotesApp.API.DTOs.Auth;

/// <summary>
/// The data the client sends when registering.
/// This is intentionally separate from the User model because:
/// - The client should never send Id or CreatedAt
/// - The client sends a plain password; the model stores a hash
/// - Validation rules belong at the boundary, not on the DB model
/// </summary>
public class RegisterRequest
{
    // [Required] means the field must be present and non-empty
    // [MinLength] / [MaxLength] guard against empty strings and DB column overflow
    [Required]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
    public string Username { get; set; } = string.Empty;

    // [EmailAddress] validates format — e.g. rejects "notanemail"
    [Required]
    [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    // Minimum 6 chars — basic protection, you'd enforce stronger rules in production
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}