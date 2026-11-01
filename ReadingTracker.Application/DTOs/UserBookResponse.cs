namespace ReadingTracker.Application.DTOs;

public record UserBookResponse(
    Guid UserBookId,
    Guid BookId,
    string Title,
    string Author,
    string Status,
    int? Rating,
    DateTime DateAdded
);