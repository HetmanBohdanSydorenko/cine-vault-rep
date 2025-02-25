using System.Diagnostics;

namespace CineVault.API.Logging;

public class TimeWatcherMiddleware : IMiddleware
{
    private readonly ILogger<TimeWatcherMiddleware> logger;

    public TimeWatcherMiddleware(ILogger<TimeWatcherMiddleware> logger)
    {
        this.logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
            
        this.logger.LogInformation("Starting request: {Method} {Path}",
            context.Request.Method, 
            context.Request.Path);
            
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            this.logger.LogWarning(ex, "Exception caught processing request: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            this.logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds
            );
        }
    }
}