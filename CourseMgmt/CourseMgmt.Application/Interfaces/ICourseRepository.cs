using CourseMgmt.Domain.Entities;
using CourseMgmt.Domain.Enums;

namespace CourseMgmt.Application.Interfaces;

/// <summary>
/// Course-specific repository interface
/// </summary>
public interface ICourseRepository : IRepository<Course>
{
    Task<(IEnumerable<Course> Courses, int TotalCount)> GetCoursesAsync(
        int page,
        int limit,
        Guid? categoryId = null,
        CourseStatus? status = null,
        CourseLevel? level = null,
        Guid? educatorId = null,
        string? search = null,
        CancellationToken cancellationToken = default);
}

