namespace CineVault.API.Controllers.Requests;

public class ApiRequest
{
    public string RequestId { get; } = Guid.NewGuid().ToString();

    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public required string UserId { get; init; }

    public required string AuthToken { get; init; }
}

public class ApiRequest<TData> : ApiRequest
{
    public required TData Data { get; init; }
}