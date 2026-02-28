using NotesApp.API.Data.Repositories;
using NotesApp.API.DTOs.Notes;
using NotesApp.API.Models;

namespace NotesApp.API.Services;

/// <summary>
/// Handles all business logic for notes.
/// The userId parameter on every method comes from the JWT token —
/// the service trusts it completely because the controller
/// extracts it from the validated token, not from user input.
/// </summary>
public class NoteService(NoteRepository noteRepository)
{
    /// <summary>
    /// Creates a new note for the given user.
    /// Returns the created note as a response DTO.
    /// </summary>
    public async Task<NoteResponse> CreateAsync(NoteRequest request, int userId)
    {
        var note = new Note
        {
            UserId = userId,        // always from the token, never from the client
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var id = await noteRepository.CreateAsync(note);

        // Map the saved note back to a response DTO
        return new NoteResponse
        {
            Id = id,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        };
    }

    /// <summary>
    /// Returns all notes for the given user as response DTOs.
    /// Mapping from Note → NoteResponse happens here in the service,
    /// so the controller and client never see the UserId field.
    /// </summary>
    // public async Task<IEnumerable<NoteResponse>> GetAllAsync(int userId)
    // {
    //     var notes = await noteRepository.GetAllByUserIdAsync(userId);

    //     return notes.Select(note => new NoteResponse
    //     {
    //         Id = note.Id,
    //         Title = note.Title,
    //         Content = note.Content,
    //         CreatedAt = note.CreatedAt,
    //         UpdatedAt = note.UpdatedAt
    //     });
    // }

    /// <summary>
    /// Returns a paged list of notes for the given user.
    /// Maps Note → NoteResponse and wraps in PagedResponse with metadata.
    /// </summary>
    public async Task<PagedResponse<NoteResponse>> GetAllAsync(
        int userId, NoteQueryParams queryParams)
    {
        var (notes, totalCount) = await noteRepository.GetAllByUserIdAsync(userId, queryParams);

        var noteResponses = notes.Select(note => new NoteResponse
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        });

        return new PagedResponse<NoteResponse>
        {
            Data = noteResponses,
            TotalCount = totalCount,
            Page = queryParams.Page,
            PageSize = queryParams.PageSize
        };
    }

    /// <summary>
    /// Gets a single note for the given user.
    /// Throws KeyNotFoundException if not found or not owned —
    /// the global exception middleware will turn this into a 404.
    /// </summary>
    public async Task<NoteResponse> GetByIdAsync(int id, int userId)
    {
        var note = await noteRepository.GetByIdAsync(id, userId);

        // We don't tell the caller WHY it wasn't found —
        // note doesn't exist and note belongs to someone else look identical
        if (note is null)
            throw new KeyNotFoundException($"Note not found.");

        return new NoteResponse
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        };
    }

    /// <summary>
    /// Updates a note for the given user.
    /// Throws KeyNotFoundException if note doesn't exist or isn't owned by this user.
    /// </summary>
    public async Task<NoteResponse> UpdateAsync(int id, NoteRequest request, int userId)
    {
        var updated = await noteRepository.UpdateAsync(
            id,
            userId,
            request.Title.Trim(),
            request.Content.Trim()
        );

        if (!updated)
            throw new KeyNotFoundException("Note not found.");

        // Fetch the updated note to return the latest state
        return await GetByIdAsync(id, userId);
    }

    /// <summary>
    /// Deletes a note for the given user.
    /// Throws KeyNotFoundException if note doesn't exist or isn't owned by this user.
    /// </summary>
    public async Task DeleteAsync(int id, int userId)
    {
        var deleted = await noteRepository.DeleteAsync(id, userId);

        if (!deleted)
            throw new KeyNotFoundException("Note not found.");
    }
}