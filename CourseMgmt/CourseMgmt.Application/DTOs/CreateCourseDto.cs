using CourseMgmt.Domain.Enums;

namespace CourseMgmt.Application.DTOs;

/// <summary>
/// DTO for creating a new course
/// </summary>
public class CreateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid EducatorId { get; set; }
    public string Duration { get; set; } = string.Empty;
    public CourseLevel Level { get; set; }
    public Guid CategoryId { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public string? Thumbnail { get; set; }
}

