namespace UserMgmt.Domain.Exceptions;

/// <summary>
/// Exception thrown when user is not authorized
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException() : base("Unauthorized access")
    {
    }
}

