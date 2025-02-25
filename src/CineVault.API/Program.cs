using CineVault.API.Extensions;
using CineVault.API.Logging;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

string environmentStr = builder.Environment.EnvironmentName;
Console.WriteLine(environmentStr);

builder.Services.AddCineVaultDbContext(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog(configuration =>
{
    string? logLevel = builder.Configuration["Logging:LogLevel:Default"];

    bool tryParse = Enum.TryParse<LogEventLevel>(logLevel, out var logLevelEnum);

    if (!tryParse)
    {
        throw new LoggingFailedException($"Invalid log level: {logLevel}");
    }

    configuration.MinimumLevel.Is(logLevelEnum)
        .WriteTo.Console()
        .WriteTo.File("Logging/log.file");
});

builder.Services.AddScoped<TimeWatcherMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsLocal())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<TimeWatcherMiddleware>();

app.MapControllers();

await app.RunAsync();