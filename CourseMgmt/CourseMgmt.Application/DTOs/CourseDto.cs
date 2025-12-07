using CourseMgmt.Domain.Enums;

namespace CourseMgmt.Application.DTOs;

/// <summary>
/// Course data transfer object
/// </summary>
public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid EducatorId { get; set; }
    public string? EducatorName { get; set; }
    public string Duration { get; set; } = string.Empty;
    public CourseLevel Level { get; set; }
    public int EnrolledCount { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public CourseStatus Status { get; set; }
    public string? Thumbnail { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

