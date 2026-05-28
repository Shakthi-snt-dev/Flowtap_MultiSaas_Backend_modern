namespace Flowtap_Application.Common.DTOs;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public List<string> Errors { get; } = [];

    protected Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
    public static Result<T> Failure(IEnumerable<string> errors)
    {
        var result = new Result<T>(false, default, null);
        result.Errors.AddRange(errors);
        return result;
    }
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public List<string> Errors { get; } = [];

    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(IEnumerable<string> errors)
    {
        var result = new Result(false, null);
        result.Errors.AddRange(errors);
        return result;
    }
}
