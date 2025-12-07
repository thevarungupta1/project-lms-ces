using System.ComponentModel.DataAnnotations;

namespace UserMgmt.Application.DTOs;

/// <summary>
/// DTO for refresh token request
/// </summary>
public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

