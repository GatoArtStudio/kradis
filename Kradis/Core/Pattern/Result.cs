namespace Kradis.Core.Pattern;

/// <summary>
/// The result pattern, which only returns the string error, receives only the string error as input and will return the same.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public string? Error { get; }

    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Ok()
        => new(true, null);

    public static Result Fail(string error)
        => new(false, error);
}

/// <summary>
/// Result pattern where the value to be received and returned is specified; the type of error received and returned is of type string.
/// </summary>
/// <typeparam name="T">The type of value it will receive and return.</typeparam>
public class Result<T> : Result
{
    public T Value { get; }

    private Result(bool success, T value, string? error)
        : base(success, error)
    {
        Value = value;
    }

    public static Result<T> Ok(T value)
        => new(true, value, null);

    public new static Result<T> Fail(string error)
        => new(false, default!, error);
}

/// <summary>
/// Result pattern that specifies the type of value and error that will be received and returned.
/// </summary>
/// <typeparam name="T">The type of value it will receive and return.</typeparam>
/// <typeparam name="TError">The type of error it will receive and return.</typeparam>
public class Result<T, TError>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private readonly T? _value;
    private readonly TError? _error;

    public T Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException();

    public TError Error =>
        IsFailure
            ? _error!
            : throw new InvalidOperationException();

    private Result(bool success, T? value, TError? error)
    {
        IsSuccess = success;
        _value = value;
        _error = error;
    }

    public static Result<T, TError> Ok(T value)
        => new(true, value, default);

    public static Result<T, TError> Fail(TError error)
        => new(false, default, error);
}