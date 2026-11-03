using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadingTracker.Application.DTOs;
using ReadingTracker.Application.Services;
using System.Security.Claims;

namespace ReadingTracker.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize] // Enforces that only logged-in users with a valid JWT can access ANY endpoint here
    public class LibraryController : ControllerBase
    {
        private readonly IUserBookService _userBookService;
        private readonly IReadingSessionService _readingSessionService;

        public LibraryController(IUserBookService userBookService, IReadingSessionService readingSessionService)
        {
            _userBookService = userBookService;
            _readingSessionService = readingSessionService;
        }

        [HttpPost]
        public async Task<IActionResult> AddBookToLibrary([FromBody] AddBookRequest request, CancellationToken cancellationToken)
        {
            // 1. Securely extract the user's ID from their JWT Token
            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            // 2. Pass the extracted ID and the DTO to the service
            var response = await _userBookService.AddBookToLibraryAsync(userId, request, cancellationToken);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyLibrary(CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var library = await _userBookService.GetUserLibraryAsync(userId, cancellationToken);

            return Ok(library);
        }

        [HttpPost("{userBookId:guid}/sessions")]
        public async Task<IActionResult> LogSession(Guid userBookId, [FromBody] LogSessionRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty) return Unauthorized();

            try
            {
                var response = await _readingSessionService.LogSessionAsync(userId, userBookId, request, cancellationToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                // Security Best Practice: Return 404 instead of 403.
                // If we return 403 Forbidden, a hacker knows the userBookId actually exists.
                // Returning 404 Not Found completely hides the existence of other users' data.
                return NotFound("Library item not found.");
            }
        }

        [HttpGet("{userBookId:guid}/sessions")]
        public async Task<IActionResult> GetSessions(Guid userBookId, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty) return Unauthorized();

            try
            {
                var sessions = await _readingSessionService.GetSessionsAsync(userId, userBookId, cancellationToken);
                return Ok(sessions);
            }
            catch (UnauthorizedAccessException)
            {
                return NotFound("Library item not found.");
            }
        }

        // Helper method to parse the Identity Claim
        private Guid GetUserIdFromToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }
    }
}

