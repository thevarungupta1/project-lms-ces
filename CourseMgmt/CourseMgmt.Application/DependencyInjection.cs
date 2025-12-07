using CourseMgmt.Application.Interfaces;
using CourseMgmt.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CourseMgmt.Application;

/// <summary>
/// Dependency injection configuration for Application layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<ICourseService, CourseService>();

        return services;
    }
}

