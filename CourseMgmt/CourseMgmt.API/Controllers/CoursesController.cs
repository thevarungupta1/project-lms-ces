using CourseMgmt.Application.DTOs;
using CourseMgmt.Application.Interfaces;
using CourseMgmt.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CourseMgmt.API.Controllers;

/// <summary>
/// Courses API controller
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
    {
        _courseService = courseService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of courses
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResultDto<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResultDto<CourseDto>>>> GetCourses(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] Guid? category = null,
        [FromQuery] CourseStatus? status = null,
        [FromQuery] CourseLevel? level = null,
        [FromQuery] Guid? educator = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _courseService.GetCoursesAsync(
            page,
            limit,
            category,
            status,
            level,
            educator,
            search,
            cancellationToken);

        return Ok(ApiResponse<PagedResultDto<CourseDto>>.CreateSuccess(result, "Courses retrieved successfully"));
    }

    /// <summary>
    /// Get course by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CourseDto>>> GetCourse(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _courseService.GetCourseByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<CourseDto>.CreateSuccess(result, "Course retrieved successfully"));
    }

    /// <summary>
    /// Create a new course
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CourseDto>>> CreateCourse(
        [FromBody] CreateCourseDto courseDto,
        CancellationToken cancellationToken = default)
    {
        var result = await _courseService.CreateCourseAsync(courseDto, cancellationToken);
        return CreatedAtAction(nameof(GetCourse), new { id = result.Id },
            ApiResponse<CourseDto>.CreateSuccess(result, "Course created successfully"));
    }

    /// <summary>
    /// Update an existing course
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CourseDto>>> UpdateCourse(
        Guid id,
        [FromBody] UpdateCourseDto courseDto,
        CancellationToken cancellationToken = default)
    {
        var result = await _courseService.UpdateCourseAsync(id, courseDto, cancellationToken);
        return Ok(ApiResponse<CourseDto>.CreateSuccess(result, "Course updated successfully"));
    }

    /// <summary>
    /// Delete a course
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCourse(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _courseService.DeleteCourseAsync(id, cancellationToken);
        return Ok(ApiResponse<object?>.CreateSuccess(null, "Course deleted successfully"));
    }
}

/// <summary>
/// Standard API response wrapper
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> CreateSuccess(T data, string message = "Operation successful")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T?> CreateFailure(string message)
    {
        return new ApiResponse<T?>
        {
            Success = false,
            Message = message
        };
    }
}
