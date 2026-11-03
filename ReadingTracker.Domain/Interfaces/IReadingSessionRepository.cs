using ReadingTracker.Domain.Entities;

namespace ReadingTracker.Domain.Interfaces;

public interface IReadingSessionRepository
{
    Task AddAsync(ReadingSession session, CancellationToken cancellationToken = default);

    // Fetches reading history for a specific book in a user's library
    Task<IEnumerable<ReadingSession>> GetSessionsForUserBookAsync(Guid userBookId, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}