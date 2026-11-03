namespace ReadingTracker.Application.DTOs;

public record ReadingSessionResponse(
    Guid Id,
    Guid UserBookId,
    int PagesRead,
    int DurationInMinutes,
    DateTime SessionDate
);