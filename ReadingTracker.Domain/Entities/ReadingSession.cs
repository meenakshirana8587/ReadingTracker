namespace ReadingTracker.Domain.Entities;

public class ReadingSession
{
    public Guid Id { get; set; }
    public Guid UserBookId { get; set; } // Links to the specific UserBook record

    public DateTime SessionDate { get; set; }
    public int PagesRead { get; set; }
    public int? DurationInMinutes { get; set; }
    public string? Notes { get; set; }

    // Navigation property
    public UserBook UserBook { get; set; } = null!;
}