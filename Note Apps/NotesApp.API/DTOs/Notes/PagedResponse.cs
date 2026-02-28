namespace NotesApp.API.DTOs.Notes;

/// <summary>
/// Wraps a list of items with pagination metadata.
/// The client needs TotalCount to know how many pages exist
/// so it can render the pagination UI correctly.
/// </summary>
public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    // Calculated server-side so the client doesn't have to
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}