using Microsoft.EntityFrameworkCore;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Domain.Interfaces;
using ReadingTracker.Infrastructure;

namespace ReadingTracker.Infrastructure.Repositories;

public class UserBookRepository : IUserBookRepository
{
    private readonly AppDbContext _dbContext;

    public UserBookRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserBook?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserBooks
            .Include(ub => ub.Book) // Eagerly load the Book data to avoid N+1 queries later
            .FirstOrDefaultAsync(ub => ub.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<UserBook>> GetUserLibraryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserBooks
            .AsNoTracking() // Massive performance boost for Read-Only queries
            .Include(ub => ub.Book)
            .Where(ub => ub.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(UserBook userBook, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserBooks.AddAsync(userBook, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}