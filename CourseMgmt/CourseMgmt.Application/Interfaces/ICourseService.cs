using CourseMgmt.Application.DTOs;

namespace CourseMgmt.Application.Interfaces;

/// <summary>
/// Course service interface for business logic operations
/// </summary>
public interface ICourseService
{
    Task<PagedResultDto<CourseDto>> GetCoursesAsync(
        int page,
        int limit,
        Guid? categoryId = null,
        Domain.Enums.CourseStatus? status = null,
        Domain.Enums.CourseLevel? level = null,
        Guid? educatorId = null,
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<CourseDto> GetCourseByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto, CancellationToken cancellationToken = default);

    Task<CourseDto> UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto, CancellationToken cancellationToken = default);

    Task DeleteCourseAsync(Guid id, CancellationToken cancellationToken = default);
}

