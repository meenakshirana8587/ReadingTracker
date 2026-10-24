using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReadingTracker.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/account")]
public class AccountController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        // Read user claims extracted by JWT middleware
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);

        return Ok(new { userId, email });
    }
}