using Microsoft.AspNetCore.Mvc;
using UserMgmt.Application.DTOs;
using UserMgmt.Application.Interfaces;

namespace UserMgmt.API.Controllers;

/// <summary>
/// Authentication API controller
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(
        [FromBody] RegisterDto registerDto,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.RegisterAsync(registerDto, cancellationToken);
        return CreatedAtAction(nameof(GetCurrentUser), null,
            ApiResponse<AuthResponseDto>.CreateSuccess(result, "User registered successfully"));
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(
        [FromBody] LoginDto loginDto,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.LoginAsync(loginDto, cancellationToken);
        return Ok(ApiResponse<AuthResponseDto>.CreateSuccess(result, "Login successful"));
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken(
        [FromBody] RefreshTokenDto refreshTokenDto,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.RefreshTokenAsync(refreshTokenDto, cancellationToken);
        return Ok(ApiResponse<AuthResponseDto>.CreateSuccess(result, "Token refreshed successfully"));
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser(
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(ApiResponse<UserDto?>.CreateFailure("Unauthorized"));
        }

        var result = await _userService.GetUserByIdAsync(userId, cancellationToken);
        return Ok(ApiResponse<UserDto>.CreateSuccess(result, "User retrieved successfully"));
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

