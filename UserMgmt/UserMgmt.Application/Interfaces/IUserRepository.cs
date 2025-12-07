using UserMgmt.Domain.Entities;

namespace UserMgmt.Application.Interfaces;

/// <summary>
/// User-specific repository interface
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}

