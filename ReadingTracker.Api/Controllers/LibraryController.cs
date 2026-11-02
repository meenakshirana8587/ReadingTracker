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

        public LibraryController(IUserBookService userBookService)
        {
            _userBookService = userBookService;
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

        // Helper method to parse the Identity Claim
        private Guid GetUserIdFromToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }
    }
}

