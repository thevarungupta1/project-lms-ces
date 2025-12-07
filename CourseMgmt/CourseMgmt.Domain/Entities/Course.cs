using CourseMgmt.Domain.Common;
using CourseMgmt.Domain.Enums;

namespace CourseMgmt.Domain.Entities;

/// <summary>
/// Course entity representing a learning course
/// </summary>
public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid EducatorId { get; set; }
    public string Duration { get; set; } = string.Empty;
    public CourseLevel Level { get; set; }
    public int EnrolledCount { get; set; } = 0;
    public Guid CategoryId { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public string? Thumbnail { get; set; }

    // Navigation properties
    public virtual User? Educator { get; set; }
    public virtual Category? Category { get; set; }
}

