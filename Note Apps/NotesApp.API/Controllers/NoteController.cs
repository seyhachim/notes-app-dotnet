using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApp.API.DTOs.Notes;
using NotesApp.API.Services;

namespace NotesApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // ← every endpoint in this controller requires a valid JWT
public class NotesController(NoteService noteService) : ControllerBase
{
    /// <summary>
    /// Extracts the userId from the JWT token claims.
    /// This is a private helper used by all endpoints in this controller.
    /// The "sub" claim is set in TokenService.GenerateToken() — it holds the user's Id.
    /// We parse it here so every action method can call GetUserId() cleanly.
    /// </summary>
    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID not found in token."));

    // ── Create ────────────────────────────────────────────────────────────────

    /// <summary>
    /// POST /api/notes
    /// Creates a new note for the authenticated user.
    /// Returns 201 Created with the new note.
    /// Returns 401 if no valid token is provided.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NoteRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var note = await noteService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(Create), new { id = note.Id }, note);
    }

    /// <summary>
    /// GET /api/notes
    /// Returns all notes for the authenticated user.
    /// Returns 401 if no valid token is provided.
    /// </summary>
    // [HttpGet]
    // public async Task<IActionResult> GetAll()
    // {
    //     var userId = GetUserId();
    //     var notes = await noteService.GetAllAsync(userId);
    //     return Ok(notes);
    // }

    /// <summary>
    /// GET /api/notes?search=keyword&sortBy=title&sortDir=asc&page=1&pageSize=10
    /// Returns a paged list of notes for the authenticated user.
    /// All query parameters are optional.
    /// [FromQuery] maps URL parameters directly to the NoteQueryParams object.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] NoteQueryParams queryParams)
    {
        var userId = GetUserId();
        var result = await noteService.GetAllAsync(userId, queryParams);
        return Ok(result);
    }


    /// <summary>
    /// GET /api/notes/{id}
    /// Returns a single note by Id.
    /// Returns 404 if not found or belongs to another user.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();
        var note = await noteService.GetByIdAsync(id, userId);
        return Ok(note);
    }

    /// <summary>
    /// PUT /api/notes/{id}
    /// Updates a note's title and content.
    /// Returns 404 if not found or belongs to another user.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] NoteRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var note = await noteService.UpdateAsync(id, request, userId);
        return Ok(note);
    }

    /// <summary>
    /// DELETE /api/notes/{id}
    /// Deletes a note.
    /// Returns 404 if not found or belongs to another user.
    /// Returns 204 No Content on success — no body needed for a delete.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        await noteService.DeleteAsync(id, userId);
        return NoContent(); // 204 — success but nothing to return
    }
}