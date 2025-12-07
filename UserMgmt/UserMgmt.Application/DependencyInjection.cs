using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserMgmt.Application.Interfaces;
using UserMgmt.Application.Services;

namespace UserMgmt.Application;

/// <summary>
/// Dependency injection configuration for Application layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Register services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService>(sp => new JwtService(configuration));

        return services;
    }
}

