using ReadingTracker.Domain.Entities;

namespace ReadingTracker.Domain.Interfaces;

public interface IUserBookRepository
{
    Task<UserBook?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Fetches the user's entire library
    Task<IEnumerable<UserBook>> GetUserLibraryAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(UserBook userBook, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}