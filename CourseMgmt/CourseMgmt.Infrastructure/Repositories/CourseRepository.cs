using CourseMgmt.Application.Interfaces;
using CourseMgmt.Domain.Entities;
using CourseMgmt.Domain.Enums;
using CourseMgmt.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseMgmt.Infrastructure.Repositories;

/// <summary>
/// Course repository implementation
/// </summary>
public class CourseRepository : Repository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Course> Courses, int TotalCount)> GetCoursesAsync(
        int page,
        int limit,
        Guid? categoryId = null,
        CourseStatus? status = null,
        CourseLevel? level = null,
        Guid? educatorId = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(c => c.Educator)
            .Include(c => c.Category)
            .AsQueryable();

        // Apply filters
        if (categoryId.HasValue)
            query = query.Where(c => c.CategoryId == categoryId.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (level.HasValue)
            query = query.Where(c => c.Level == level.Value);

        if (educatorId.HasValue)
            query = query.Where(c => c.EducatorId == educatorId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => 
                c.Title.Contains(search) || 
                c.Description.Contains(search));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination and ordering
        var courses = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return (courses, totalCount);
    }

    public override async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Educator)
            .Include(c => c.Category)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}

