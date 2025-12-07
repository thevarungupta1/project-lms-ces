using CourseMgmt.Domain.Common;

namespace CourseMgmt.Domain.Entities;

/// <summary>
/// User entity representing system users (educators, admins, learners)
/// </summary>
public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // admin, educator, learner
    public string? Department { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
}

