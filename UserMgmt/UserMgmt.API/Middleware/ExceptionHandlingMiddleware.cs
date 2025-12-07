using System.Net;
using System.Text.Json;
using UserMgmt.Domain.Exceptions;

namespace UserMgmt.API.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object response;
        int statusCode;

        switch (exception)
        {
            case NotFoundException notFound:
                response = new
                {
                    Success = false,
                    Message = notFound.Message
                };
                statusCode = (int)HttpStatusCode.NotFound;
                break;

            case ValidationException validation:
                response = new
                {
                    Success = false,
                    Message = validation.Message,
                    Errors = validation.Errors
                };
                statusCode = (int)HttpStatusCode.BadRequest;
                break;

            case BusinessRuleException businessRule:
                response = new
                {
                    Success = false,
                    Message = businessRule.Message
                };
                statusCode = (int)HttpStatusCode.BadRequest;
                break;

            case UnauthorizedException unauthorized:
                response = new
                {
                    Success = false,
                    Message = unauthorized.Message
                };
                statusCode = (int)HttpStatusCode.Unauthorized;
                break;

            default:
                response = new
                {
                    Success = false,
                    Message = "An error occurred while processing your request."
                };
                statusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.StatusCode = statusCode;

        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}

