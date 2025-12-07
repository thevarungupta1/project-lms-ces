using CourseMgmt.Domain.Enums;

namespace CourseMgmt.Application.DTOs;

/// <summary>
/// DTO for updating an existing course
/// </summary>
public class UpdateCourseDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Duration { get; set; }
    public CourseLevel? Level { get; set; }
    public Guid? CategoryId { get; set; }
    public CourseStatus? Status { get; set; }
    public string? Thumbnail { get; set; }
}

