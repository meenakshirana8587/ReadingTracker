using System.ComponentModel.DataAnnotations;

namespace ReadingTracker.Domain.Entities;

public enum ReadingStatus
{
    PlanToRead,

    [Display(Name = "In Progress")] // for future to see if we want to display it differently in UI
    Reading,
    Completed,
    Dropped
}