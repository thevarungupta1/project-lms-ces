using CourseMgmt.Domain.Common;

namespace CourseMgmt.Domain.Entities;

/// <summary>
/// Category entity for organizing courses
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#3b82f6";
    public bool IsActive { get; set; } = true;
}

