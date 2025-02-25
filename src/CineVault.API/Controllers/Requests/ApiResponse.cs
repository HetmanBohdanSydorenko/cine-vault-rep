namespace CineVault.API.Controllers.Requests;

public class ApiResponse
{
    public required bool Success { get; init; }

    public required string RequestId { get; init; }

    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public required string Message { get; init; }

    public required int StatusCode { get; init; }

    public List<string> Errors { get; } = new List<string>();
}

public class ApiResponse<TData> : ApiResponse
{
    public required TData Data { get; init; }
}