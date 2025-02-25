using Asp.Versioning;
using CineVault.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

string environmentStr = builder.Environment.EnvironmentName;
Console.WriteLine(environmentStr);

builder.Services.AddCineVaultDbContext(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CineVault API", Version = "1" });
    options.SwaggerDoc("v2", new OpenApiInfo { Title = "CineVault API", Version = "2" });
});

builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("v1/swagger.json", "v1");
        options.SwaggerEndpoint("v2/swagger.json", "v2");
    });
}

if (app.Environment.IsLocal())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();