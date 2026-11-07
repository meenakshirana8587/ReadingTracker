namespace ReadingTracker.Domain.Entities;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public int TotalPages { get; set; }
    public string? Genre { get; set; }
    public int? Rating { get; set; } // Optional rating out of 5

    // Navigation property for EF Core
    public ICollection<UserBook> UserBooks { get; set; } = new List<UserBook>();
}