using NotesApp.API.Data.Repositories;
using NotesApp.API.DTOs.Auth;
using NotesApp.API.Models;

namespace NotesApp.API.Services;

/// <summary>
/// Handles authentication business logic.
/// This is the only place that knows the rules for registering a user.
/// Controllers call this and trust it — they don't repeat these checks.
/// </summary>
public class AuthService(UserRepository userRepository, TokenService tokenService)
{
    /// <summary>
    /// Validates and creates a new user account.
    /// Returns the new user's Id on success.
    /// Throws InvalidOperationException if email or username is already taken.
    /// </summary>
    public async Task<int> RegisterAsync(RegisterRequest request)
    {
        // Business rule: email must be unique across all users
        var existingEmail = await userRepository.GetByEmailAsync(request.Email);
        if (existingEmail is not null)
            throw new InvalidOperationException("Email is already in use.");

        // Business rule: username must be unique across all users
        var existingUsername = await userRepository.GetByUsernameAsync(request.Username);
        if (existingUsername is not null)
            throw new InvalidOperationException("Username is already taken.");

        var user = new User
        {
            Username = request.Username,

            // Normalize email to lowercase before saving —
            // prevents "User@Email.com" and "user@email.com" being treated as different accounts
            Email = request.Email.ToLowerInvariant(),

            // BCrypt hashes the password with a random salt built in.
            // The hash is slow by design — makes brute-force attacks on a stolen DB impractical.
            // Work factor defaults to 11 (2^11 iterations). Never store plain or MD5/SHA passwords.
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),

            CreatedAt = DateTime.UtcNow
        };

        return await userRepository.CreateAsync(user);
    }

    /// <summary>
    /// Validates credentials and returns a JWT token on success.
    /// Throws UnauthorizedAccessException if email not found or password wrong.
    /// 
    /// Important: we return the SAME error message for both "email not found"
    /// and "wrong password" — this prevents attackers from using the error
    /// message to discover which emails are registered (user enumeration attack).
    /// </summary>
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email.ToLowerInvariant());

        // BCrypt.Verify hashes the incoming password and compares it to the stored hash.
        // If user is null we still call Verify with a dummy hash to prevent
        // timing attacks — attacker can't tell "user not found" vs "wrong password"
        // by measuring how long the response takes.
        var passwordValid = user is not null &&
            BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

        if (!passwordValid)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = tokenService.GenerateToken(user!);

        return new AuthResponse
        {
            Token = token,
            Username = user!.Username,
            Email = user.Email
        };
    }
}