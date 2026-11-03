using Microsoft.EntityFrameworkCore;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Domain.Interfaces;

namespace ReadingTracker.Infrastructure.Repositories;

public class ReadingSessionRepository : IReadingSessionRepository
{
    private readonly AppDbContext _dbContext;

    public ReadingSessionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ReadingSession session, CancellationToken cancellationToken = default)
    {
        await _dbContext.ReadingSessions.AddAsync(session, cancellationToken);
    }

    public async Task<IEnumerable<ReadingSession>> GetSessionsForUserBookAsync(Guid userBookId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ReadingSessions
            .AsNoTracking() // Optimized for read-only history rendering
            .Where(rs => rs.UserBookId == userBookId)
            .OrderByDescending(rs => rs.SessionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}