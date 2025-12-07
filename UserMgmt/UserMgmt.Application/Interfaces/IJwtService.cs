using UserMgmt.Domain.Entities;

namespace UserMgmt.Application.Interfaces;

/// <summary>
/// JWT service interface for token generation and validation
/// </summary>
public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Guid? ValidateToken(string token);
}

