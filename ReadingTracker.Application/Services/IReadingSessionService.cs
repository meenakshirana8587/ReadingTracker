using ReadingTracker.Application.DTOs;

namespace ReadingTracker.Application.Services;

public interface IReadingSessionService
{
    Task<ReadingSessionResponse> LogSessionAsync(Guid userId, Guid userBookId, LogSessionRequest request, CancellationToken cancellationToken = default);

    Task<IEnumerable<ReadingSessionResponse>> GetSessionsAsync(Guid userId, Guid userBookId, CancellationToken cancellationToken = default);
}