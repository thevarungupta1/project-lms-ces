using CourseMgmt.Application.DTOs;
using CourseMgmt.Application.Interfaces;
using CourseMgmt.Domain.Entities;
using CourseMgmt.Domain.Enums;
using CourseMgmt.Domain.Exceptions;

namespace CourseMgmt.Application.Services;

/// <summary>
/// Course service implementation for business logic operations
/// </summary>
public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Category> _categoryRepository;

    public CourseService(
        ICourseRepository courseRepository,
        IRepository<User> userRepository,
        IRepository<Category> categoryRepository)
    {
        _courseRepository = courseRepository;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResultDto<CourseDto>> GetCoursesAsync(
        int page,
        int limit,
        Guid? categoryId = null,
        CourseStatus? status = null,
        CourseLevel? level = null,
        Guid? educatorId = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var (courses, totalCount) = await _courseRepository.GetCoursesAsync(
            page,
            limit,
            categoryId,
            status,
            level,
            educatorId,
            search,
            cancellationToken);

        var courseDtos = courses.Select(course => new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            EducatorId = course.EducatorId,
            EducatorName = course.Educator?.Name,
            Duration = course.Duration,
            Level = course.Level,
            EnrolledCount = course.EnrolledCount,
            CategoryId = course.CategoryId,
            CategoryName = course.Category?.Name,
            Status = course.Status,
            Thumbnail = course.Thumbnail,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        });

        return new PagedResultDto<CourseDto>
        {
            Data = courseDtos,
            Page = page,
            Limit = limit,
            Total = totalCount
        };
    }

    public async Task<CourseDto> GetCourseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken);

        if (course == null)
        {
            throw new NotFoundException(nameof(Course), id);
        }

        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            EducatorId = course.EducatorId,
            EducatorName = course.Educator?.Name,
            Duration = course.Duration,
            Level = course.Level,
            EnrolledCount = course.EnrolledCount,
            CategoryId = course.CategoryId,
            CategoryName = course.Category?.Name,
            Status = course.Status,
            Thumbnail = course.Thumbnail,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto, CancellationToken cancellationToken = default)
    {
        // Verify educator exists
        var educator = await _userRepository.GetByIdAsync(createCourseDto.EducatorId, cancellationToken);
        if (educator == null)
        {
            throw new NotFoundException(nameof(User), createCourseDto.EducatorId);
        }

        // Verify category exists
        var category = await _categoryRepository.GetByIdAsync(createCourseDto.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), createCourseDto.CategoryId);
        }

        // Create course entity
        var course = new Course
        {
            Title = createCourseDto.Title,
            Description = createCourseDto.Description,
            EducatorId = createCourseDto.EducatorId,
            Duration = createCourseDto.Duration,
            Level = createCourseDto.Level,
            CategoryId = createCourseDto.CategoryId,
            Status = createCourseDto.Status,
            Thumbnail = createCourseDto.Thumbnail,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _courseRepository.AddAsync(course, cancellationToken);

        // Reload to get navigation properties
        course = await _courseRepository.GetByIdAsync(course.Id, cancellationToken) 
            ?? throw new Domain.Exceptions.NotFoundException(nameof(Course), course.Id);

        return new CourseDto
        {
            Id = course!.Id,
            Title = course.Title,
            Description = course.Description,
            EducatorId = course.EducatorId,
            EducatorName = course.Educator?.Name,
            Duration = course.Duration,
            Level = course.Level,
            EnrolledCount = course.EnrolledCount,
            CategoryId = course.CategoryId,
            CategoryName = course.Category?.Name,
            Status = course.Status,
            Thumbnail = course.Thumbnail,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }

    public async Task<CourseDto> UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto, CancellationToken cancellationToken = default)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken);

        if (course == null)
        {
            throw new NotFoundException(nameof(Course), id);
        }

        // Verify category if being updated
        if (updateCourseDto.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(updateCourseDto.CategoryId.Value, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException(nameof(Category), updateCourseDto.CategoryId.Value);
            }
        }

        // Update course properties
        if (!string.IsNullOrWhiteSpace(updateCourseDto.Title))
            course.Title = updateCourseDto.Title;

        if (!string.IsNullOrWhiteSpace(updateCourseDto.Description))
            course.Description = updateCourseDto.Description;

        if (!string.IsNullOrWhiteSpace(updateCourseDto.Duration))
            course.Duration = updateCourseDto.Duration;

        if (updateCourseDto.Level.HasValue)
            course.Level = updateCourseDto.Level.Value;

        if (updateCourseDto.CategoryId.HasValue)
            course.CategoryId = updateCourseDto.CategoryId.Value;

        if (updateCourseDto.Status.HasValue)
            course.Status = updateCourseDto.Status.Value;

        if (updateCourseDto.Thumbnail != null)
            course.Thumbnail = updateCourseDto.Thumbnail;

        course.UpdatedAt = DateTime.UtcNow;

        await _courseRepository.UpdateAsync(course, cancellationToken);

        // Reload to get navigation properties
        course = await _courseRepository.GetByIdAsync(id, cancellationToken);

        return new CourseDto
        {
            Id = course!.Id,
            Title = course.Title,
            Description = course.Description,
            EducatorId = course.EducatorId,
            EducatorName = course.Educator?.Name,
            Duration = course.Duration,
            Level = course.Level,
            EnrolledCount = course.EnrolledCount,
            CategoryId = course.CategoryId,
            CategoryName = course.Category?.Name,
            Status = course.Status,
            Thumbnail = course.Thumbnail,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }

    public async Task DeleteCourseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken);

        if (course == null)
        {
            throw new NotFoundException(nameof(Course), id);
        }

        await _courseRepository.DeleteAsync(course, cancellationToken);
    }
}

