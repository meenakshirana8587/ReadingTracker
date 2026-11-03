using System.ComponentModel.DataAnnotations;

namespace ReadingTracker.Application.DTOs;

public record LogSessionRequest(
    [Required][Range(1, 1000, ErrorMessage = "Pages read must be between 1 and 1000.")] int PagesRead,
    [Required][Range(1, 1440, ErrorMessage = "Duration must be between 1 minute and 24 hours.")] int DurationInMinutes,
    DateTime? SessionDate // Optional: If null, we default to DateTime.UtcNow in the service
);