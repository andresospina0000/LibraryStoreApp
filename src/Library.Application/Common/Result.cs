namespace Library.Application.Common;

/// <summary>
/// Standardized response envelope. Flags whether the operation succeeded,
/// carries a human-readable error and an <see cref="ErrorType"/> for HTTP mapping.
/// </summary>
public class Result
{
    public bool Succeeded { get; protected set; }

    public string? Error { get; protected set; }

    public ErrorType ErrorType { get; protected set; } = ErrorType.None;

    public static Result Success() => new() { Succeeded = true };

    public static Result Failure(string error, ErrorType type = ErrorType.ServerError) =>
        new() { Succeeded = false, Error = error, ErrorType = type };
}

/// <summary>
/// Standardized response envelope carrying a payload of type <typeparamref name="T"/>.
/// </summary>
public class Result<T> : Result
{
    public T? Data { get; private set; }

    public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };

    public static new Result<T> Failure(string error, ErrorType type = ErrorType.ServerError) =>
        new() { Succeeded = false, Error = error, ErrorType = type };
}
