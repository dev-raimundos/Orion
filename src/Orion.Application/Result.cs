namespace Orion.Application;

public enum ErrorType
{
    None,
    Validation,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    Unexpected
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ErrorType ErrorType { get; }

    private Result(bool isSuccess, T? value, string? error, ErrorType errorType)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ErrorType = errorType;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null, ErrorType.None);
    }

    public static Result<T> Failure(string error, ErrorType errorType)
    {
        return new Result<T>(false, default, error, errorType);
    }

}