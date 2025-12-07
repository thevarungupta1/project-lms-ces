using System.Security.Cryptography;
using System.Text;
using UserMgmt.Application.DTOs;
using UserMgmt.Application.Interfaces;
using UserMgmt.Domain.Entities;
using UserMgmt.Domain.Exceptions;

namespace UserMgmt.Application.Services;

/// <summary>
/// User service implementation for business logic operations
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public UserService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(registerDto.Email, cancellationToken))
        {
            throw new BusinessRuleException("Email is already registered.");
        }

        // Hash password
        var passwordHash = HashPassword(registerDto.Password);

        // Create user entity
        var user = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = registerDto.Role,
            Department = registerDto.Department,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Department = user.Department,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email.ToLowerInvariant(), cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException("Account is inactive.");
        }

        if (!VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Department = user.Department,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(u => 
            u.RefreshToken == refreshTokenDto.RefreshToken &&
            u.RefreshTokenExpiryTime > DateTime.UtcNow, cancellationToken);

        var userEntity = user.FirstOrDefault();

        if (userEntity == null)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        if (!userEntity.IsActive)
        {
            throw new UnauthorizedException("Account is inactive.");
        }

        // Generate new tokens
        var accessToken = _jwtService.GenerateAccessToken(userEntity);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Update refresh token
        userEntity.RefreshToken = refreshToken;
        userEntity.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        userEntity.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(userEntity, cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = userEntity.Id,
                Name = userEntity.Name,
                Email = userEntity.Email,
                Role = userEntity.Role,
                Department = userEntity.Department,
                IsActive = userEntity.IsActive,
                LastLoginAt = userEntity.LastLoginAt,
                CreatedAt = userEntity.CreatedAt,
                UpdatedAt = userEntity.UpdatedAt
            }
        };
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Department = user.Department,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<UserDto> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email.ToLowerInvariant(), cancellationToken);

        if (user == null)
        {
            throw new NotFoundException($"User with email '{email}' not found.");
        }

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Department = user.Department,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == passwordHash;
    }
}

