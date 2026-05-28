namespace Flowtap_Application.Common.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = [];
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string error, string? traceId = null) =>
        new() { Success = false, Errors = [error], TraceId = traceId };

    public static ApiResponse<T> Fail(IEnumerable<string> errors, string? traceId = null) =>
        new() { Success = false, Errors = [..errors], TraceId = traceId };
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Ok(string? message = null) =>
        new() { Success = true, Message = message };
}
