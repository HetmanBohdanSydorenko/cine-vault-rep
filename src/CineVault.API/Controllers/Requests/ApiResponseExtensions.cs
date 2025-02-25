namespace CineVault.API.Controllers.Requests;

public static class ApiResponseExtensions
{
    public static ApiResponse ToApiResponse(this ApiRequest request, bool success, string message, int statusCode)
    {
        return new ApiResponse
        {
            Success = success,
            RequestId = request.RequestId,
            Message = message,
            StatusCode = statusCode
        };
    }
    
    public static ApiResponse<T> ToApiResponse<T>(this ApiRequest request, bool success, string message, int statusCode, T data)
    {
        return new ApiResponse<T>
        {
            Success = success,
            RequestId = request.RequestId,
            Message = message,
            StatusCode = statusCode,
            Data = data
        };
    }
}