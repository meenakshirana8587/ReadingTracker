using ReadingTracker.Application.DTOs;

namespace ReadingTracker.Application.Services;

public interface IUserBookService
{
    Task<UserBookResponse> AddBookToLibraryAsync(Guid userId, AddBookRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserBookResponse>> GetUserLibraryAsync(Guid userId, CancellationToken cancellationToken = default);
}