using ReadingTracker.Application.DTOs;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Domain.Interfaces;

namespace ReadingTracker.Application.Services;

public class ReadingSessionService : IReadingSessionService
{
    private readonly IReadingSessionRepository _sessionRepository;
    private readonly IUserBookRepository _userBookRepository;

    public ReadingSessionService(
        IReadingSessionRepository sessionRepository,
        IUserBookRepository userBookRepository)
    {
        _sessionRepository = sessionRepository;
        _userBookRepository = userBookRepository;
    }

    public async Task<ReadingSessionResponse> LogSessionAsync(Guid userId, Guid userBookId, LogSessionRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Cross-Tenant Security Check (BOLA Prevention)
        var userBook = await _userBookRepository.GetByIdAsync(userBookId, cancellationToken);

        if (userBook == null || userBook.UserId != userId)
        {
            // If the book doesn't exist OR doesn't belong to the logged-in user, reject it.
            // In a production app, you might throw a custom NotFoundException or ForbiddenException here.
            throw new UnauthorizedAccessException("You do not have permission to modify this library item.");
        }

        // 2. Map DTO to Domain Entity
        var session = new ReadingSession
        {
            Id = Guid.NewGuid(),
            UserBookId = userBookId,
            PagesRead = request.PagesRead,
            DurationInMinutes = request.DurationInMinutes,
            SessionDate = request.SessionDate ?? DateTime.UtcNow
        };

        // 3. Persist to Database
        await _sessionRepository.AddAsync(session, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        // 4. Return Response DTO
        return new ReadingSessionResponse(
            session.Id,
            session.UserBookId,
            session.PagesRead,
            session.DurationInMinutes ?? default,
            session.SessionDate
        );
    }

    public async Task<IEnumerable<ReadingSessionResponse>> GetSessionsAsync(Guid userId, Guid userBookId, CancellationToken cancellationToken = default)
    {
        // Re-apply the security check to ensure they can't view someone else's reading history
        var userBook = await _userBookRepository.GetByIdAsync(userBookId, cancellationToken);
        if (userBook == null || userBook.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to view this library item.");
        }

        var sessions = await _sessionRepository.GetSessionsForUserBookAsync(userBookId, cancellationToken);

        return sessions.Select(s => new ReadingSessionResponse(
            s.Id,
            s.UserBookId,
            s.PagesRead,
            s.DurationInMinutes ?? default,
            s.SessionDate
        ));
    }
}