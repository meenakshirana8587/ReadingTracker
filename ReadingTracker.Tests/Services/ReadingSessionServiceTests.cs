using Moq;
using ReadingTracker.Application.DTOs;
using ReadingTracker.Application.Services;
using ReadingTracker.Domain.Entities;
using ReadingTracker.Domain.Interfaces;

namespace ReadingTracker.Tests.Services;

[TestFixture]
public class ReadingSessionServiceTests
{
    private Mock<IReadingSessionRepository> _sessionRepositoryMock;
    private Mock<IUserBookRepository> _userBookRepositoryMock;
    private ReadingSessionService _sut; // SUT = System Under Test

    [SetUp]
    public void Setup()
    {
        // Re-initialize mocks before every single test to ensure isolation
        _sessionRepositoryMock = new Mock<IReadingSessionRepository>();
        _userBookRepositoryMock = new Mock<IUserBookRepository>();

        _sut = new ReadingSessionService(_sessionRepositoryMock.Object, _userBookRepositoryMock.Object);
    }

    [Test]
    public void LogSessionAsync_WhenUserDoesNotOwnBook_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var hackerUserId = Guid.NewGuid();
        var victimUserBookId = Guid.NewGuid();
        var request = new LogSessionRequest(50, 45, DateTime.UtcNow);

        var victimUserBook = new UserBook
        {
            Id = victimUserBookId,
            UserId = Guid.NewGuid() // Belongs to a different user!
        };

        _userBookRepositoryMock
            .Setup(repo => repo.GetByIdAsync(victimUserBookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(victimUserBook);

        // Act & Assert
        var exception = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _sut.LogSessionAsync(hackerUserId, victimUserBookId, request));

        Assert.That(exception.Message, Is.EqualTo("You do not have permission to modify this library item."));

        // Verify the database was NEVER called to save anything
        _sessionRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ReadingSession>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task LogSessionAsync_WhenUserOwnsBook_SavesSessionAndReturnsResponse()
    {
        // Arrange
        var myUserId = Guid.NewGuid();
        var myUserBookId = Guid.NewGuid();
        var request = new LogSessionRequest(50, 45, DateTime.UtcNow);

        var myUserBook = new UserBook
        {
            Id = myUserBookId,
            UserId = myUserId
        };

        _userBookRepositoryMock
            .Setup(repo => repo.GetByIdAsync(myUserBookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(myUserBook);

        // Act
        var result = await _sut.LogSessionAsync(myUserId, myUserBookId, request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PagesRead, Is.EqualTo(request.PagesRead));
        Assert.That(result.DurationInMinutes, Is.EqualTo(request.DurationInMinutes));

        // Verify the repository was explicitly called exactly once to save the data
        _sessionRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ReadingSession>(), It.IsAny<CancellationToken>()), Times.Once);
        _sessionRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}