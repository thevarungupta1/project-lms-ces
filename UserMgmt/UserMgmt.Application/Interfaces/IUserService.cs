using UserMgmt.Application.DTOs;

namespace UserMgmt.Application.Interfaces;

/// <summary>
/// User service interface for business logic operations
/// </summary>
public interface IUserService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken = default);
    Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
}

