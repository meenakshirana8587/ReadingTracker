using ReadingTracker.Application.DTOs;
using ReadingTracker.Application.Services;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Domain.Interfaces;

namespace ReadingTracker.Application.Services;

public class UserBookService : IUserBookService
{
    private readonly IUserBookRepository _userBookRepository;

    public UserBookService(IUserBookRepository userBookRepository)
    {
        _userBookRepository = userBookRepository;
    }

    public async Task<UserBookResponse> AddBookToLibraryAsync(Guid userId, AddBookRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Map the DTO to the Domain Entity
        var userBook = new UserBook
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BookId = request.BookId,
            Status = request.Status,
            DateAdded = DateTime.UtcNow
        };

        // 2. Save using the Repository
        await _userBookRepository.AddAsync(userBook, cancellationToken);
        await _userBookRepository.SaveChangesAsync(cancellationToken);

        // 3. Fetch the saved entity (with the Book navigation property included) to return a rich DTO
        var savedBook = await _userBookRepository.GetByIdAsync(userBook.Id, cancellationToken);

        return new UserBookResponse(
            savedBook!.Id,
            savedBook.BookId,
            savedBook.Book.Title,
            savedBook.Book.Author,
            savedBook.Status.ToString(),
            savedBook.Rating,
            savedBook.DateAdded
        );
    }

    public async Task<IEnumerable<UserBookResponse>> GetUserLibraryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var library = await _userBookRepository.GetUserLibraryAsync(userId, cancellationToken);

        // Map the list of entities to a list of DTOs
        return library.Select(ub => new UserBookResponse(
            ub.Id,
            ub.BookId,
            ub.Book.Title,
            ub.Book.Author,
            ub.Status.ToString(),
            ub.Rating,
            ub.DateAdded
        ));
    }
}