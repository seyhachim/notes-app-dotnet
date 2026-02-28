using Dapper;
using NotesApp.API.Models;

namespace NotesApp.API.Data.Repositories;

using NotesApp.API.DTOs.Notes;

/// <summary>
/// Handles all database operations for the Notes table.
/// Every query includes UserId in the WHERE clause —
/// this ensures a user can never accidentally access another user's notes,
/// even if they somehow guess the note Id.
/// </summary>
public class NoteRepository(DbConnectionFactory connectionFactory)
{
    /// <summary>
    /// Inserts a new note and returns the generated Id.
    /// </summary>
    public async Task<int> CreateAsync(Note note)
    {
        using var connection = connectionFactory.Create();
        return await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
            OUTPUT INSERTED.Id
            VALUES (@UserId, @Title, @Content, @CreatedAt, @UpdatedAt)
            """,
            note
        );
    }

    /// <summary>
    /// Returns all notes belonging to the given user.
    /// UserId is always in the WHERE clause — a user can never
    /// see another user's notes even if they guess the note Id.
    /// </summary>
    // public async Task<IEnumerable<Note>> GetAllByUserIdAsync(int userId)
    // {
    //     using var connection = connectionFactory.Create();
    //     return await connection.QueryAsync<Note>(
    //         """
    //     SELECT Id, Title, Content, CreatedAt, UpdatedAt
    //     FROM Notes
    //     WHERE UserId = @UserId
    //     ORDER BY CreatedAt DESC
    //     """,
    //         new { UserId = userId }
    //     );
    // }

    /// <summary>
    /// Returns a paged, filtered, sorted list of notes for a user.
    /// Everything happens in SQL — no in-memory filtering.
    /// 
    /// Two queries run together using Dapper's QueryMultipleAsync:
    /// 1. The actual notes for this page
    /// 2. The total count for pagination metadata
    /// Running them together saves a round trip to the database.
    /// </summary>
    public async Task<(IEnumerable<Note> Notes, int TotalCount)> GetAllByUserIdAsync(
        int userId, NoteQueryParams queryParams)
    {
        // Validate sortBy to prevent SQL injection on the ORDER BY clause.
        // We can't parameterize column names in SQL, so we whitelist them instead.
        var allowedSortColumns = new[] { "title", "createdAt", "updatedAt" };
        var sortBy = allowedSortColumns.Contains(queryParams.SortBy.ToLower())
            ? queryParams.SortBy
            : "createdAt";

        var sortDir = queryParams.SortDir.ToLower() == "asc" ? "ASC" : "DESC";

        // Map camelCase param to actual SQL column name
        var sortColumn = sortBy switch
        {
            "title" => "Title",
            "updatedAt" => "UpdatedAt",
            _ => "CreatedAt"
        };

        var sql = $"""
        SELECT Id, UserId, Title, Content, CreatedAt, UpdatedAt
        FROM Notes
        WHERE UserId = @UserId
          AND (@Search IS NULL OR 
               Title LIKE '%' + @Search + '%' OR 
               Content LIKE '%' + @Search + '%')
        ORDER BY {sortColumn} {sortDir}
        OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(*)
        FROM Notes
        WHERE UserId = @UserId
          AND (@Search IS NULL OR 
               Title LIKE '%' + @Search + '%' OR 
               Content LIKE '%' + @Search + '%');
        """;

        // Note: sortColumn and sortDir are NOT user input —
        // they come from the whitelist above, so interpolation is safe here

        var parameters = new
        {
            UserId = userId,
            Search = string.IsNullOrWhiteSpace(queryParams.Search)
                        ? null
                        : queryParams.Search.Trim(),
            Skip = queryParams.Skip,
            PageSize = queryParams.PageSize
        };

        using var connection = connectionFactory.Create();

        // QueryMultipleAsync executes both queries in one round trip
        using var multi = await connection.QueryMultipleAsync(sql, parameters);

        var notes = await multi.ReadAsync<Note>();
        var totalCount = await multi.ReadFirstAsync<int>();

        return (notes, totalCount);
    }

    /// <summary>
    /// Gets a single note by Id AND UserId.
    /// The UserId check is the security guard —
    /// if the note exists but belongs to someone else, this returns null.
    /// The caller gets a 404 either way — we never confirm a note exists to the wrong user.
    /// </summary>
    public async Task<Note?> GetByIdAsync(int id, int userId)
    {
        using var connection = connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<Note>(
            """
        SELECT Id, Title, Content, CreatedAt, UpdatedAt
        FROM Notes
        WHERE Id = @Id AND UserId = @UserId
        """,
            new { Id = id, UserId = userId }
        );
    }

    /// <summary>
    /// Updates Title and Content of an existing note.
    /// UpdatedAt is stamped here — the client never sends it.
    /// WHERE Id AND UserId ensures a user can only update their own notes.
    /// </summary>
    public async Task<bool> UpdateAsync(int id, int userId, string title, string content)
    {
        using var connection = connectionFactory.Create();
        var rows = await connection.ExecuteAsync(
            """
        UPDATE Notes
        SET Title = @Title,
            Content = @Content,
            UpdatedAt = @UpdatedAt
        WHERE Id = @Id AND UserId = @UserId
        """,
            new { Id = id, UserId = userId, Title = title, Content = content, UpdatedAt = DateTime.UtcNow }
        );

        // ExecuteAsync returns the number of rows affected.
        // If 0 rows were updated, the note either doesn't exist or belongs to someone else.
        return rows > 0;
    }

    /// <summary>
    /// Deletes a note by Id AND UserId.
    /// Same pattern — 0 rows affected means not found or not owned.
    /// </summary>
    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var connection = connectionFactory.Create();
        var rows = await connection.ExecuteAsync(
            "DELETE FROM Notes WHERE Id = @Id AND UserId = @UserId",
            new { Id = id, UserId = userId }
        );
        return rows > 0;
    }
}