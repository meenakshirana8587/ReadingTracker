using System.ComponentModel.DataAnnotations;
using ReadingTracker.Domain.Entities;

namespace ReadingTracker.Application.DTOs;

public record AddBookRequest(
    [Required] Guid BookId,
    ReadingStatus Status = ReadingStatus.PlanToRead
);