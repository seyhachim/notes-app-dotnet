using Dapper;
using NotesApp.API.Models;

namespace NotesApp.API.Data.Repositories;

/// <summary>
/// Handles all database operations for the Users table.
/// This class only speaks SQL — no business logic lives here.
/// Business rules (e.g. "email must be unique") belong in AuthService.
/// </summary>
public class UserRepository(DbConnectionFactory connectionFactory)
{
    /// <summary>
    /// Looks up a user by email. Returns null if not found.
    /// Used during registration to check for duplicates,
    /// and during login to find the user to authenticate.
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email)
    {
        // 'using' ensures the connection is closed and returned to the
        // connection pool immediately after the query completes
        using var connection = connectionFactory.Create();

        // QuerySingleOrDefaultAsync returns null if no row matches —
        // safer than QuerySingle which throws if nothing is found
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email",
            new { Email = email }  // Dapper binds this as a parameter — NOT string concatenation
        );
    }

    /// <summary>
    /// Looks up a user by username. Returns null if not found.
    /// Used during registration to prevent duplicate usernames.
    /// </summary>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Username = @Username",
            new { Username = username }
        );
    }

    /// <summary>
    /// Inserts a new user and returns the generated Id.
    /// The user object passed in already has the password hashed —
    /// this method never sees or handles plain text passwords.
    /// </summary>
    public async Task<int> CreateAsync(User user)
    {
        using var connection = connectionFactory.Create();

        return await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO Users (Username, Email, Password, CreatedAt)
            OUTPUT INSERTED.Id
            VALUES (@Username, @Email, @Password, @CreatedAt)
            """,
            user  // Dapper maps each property to its matching @Parameter by name
                  // OUTPUT INSERTED.Id returns the new row's Id in the same round trip —
                  // avoids a second SELECT query to fetch the Id
        );
    }
}