namespace NotesApp.API.DTOs.Notes;

/// <summary>
/// Query parameters the client sends to filter, sort and paginate notes.
/// All fields are optional — the API works with or without them.
/// Defaults are defined here so the controller and service never deal with nulls.
/// </summary>
public class NoteQueryParams
{
    // Search term — matches against Title and Content
    public string? Search { get; set; }

    // Which column to sort by — "title", "createdAt", "updatedAt"
    // Defaults to createdAt
    public string SortBy { get; set; } = "createdAt";

    // Sort direction — "asc" or "desc"
    public string SortDir { get; set; } = "desc";

    // Which page the client wants — 1-based
    public int Page { get; set; } = 1;

    // How many notes per page — capped at 50 to prevent abuse
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 50 ? 50 : value < 1 ? 1 : value;
    }

    // Calculated by the API — never sent by the client
    // OFFSET tells SQL how many rows to skip
    public int Skip => (Page - 1) * PageSize;
}