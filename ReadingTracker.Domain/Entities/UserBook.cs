namespace ReadingTracker.Domain.Entities;

public class UserBook
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } // Links to ApplicationUser (Identity)
    public Guid BookId { get; set; } // Links to Book

    public ReadingStatus Status { get; set; } = ReadingStatus.PlanToRead;
    public int? Rating { get; set; } // e.g., 1-5 scale
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Book Book { get; set; } = null!;
    public ICollection<ReadingSession> ReadingSessions { get; set; } = new List<ReadingSession>();
}