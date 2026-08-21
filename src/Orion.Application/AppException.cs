namespace Orion.Application;

public enum ErrorType
{
    None,
    Validation,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    Locked,
    Unexpected
}

public abstract class AppException(string message, ErrorType errorType) : Exception(message)
{
    public ErrorType ErrorType { get; } = errorType;
}

public sealed class AppValidationException(string message) : AppException(message, ErrorType.Validation);
public sealed class AppUnauthorizedException(string message) : AppException(message, ErrorType.Unauthorized);
public sealed class AppForbiddenException(string message) : AppException(message, ErrorType.Forbidden);
public sealed class AppNotFoundException(string message) : AppException(message, ErrorType.NotFound);
public sealed class AppConflictException(string message) : AppException(message, ErrorType.Conflict);
public sealed class AppLockedException(string message) : AppException(message, ErrorType.Locked);