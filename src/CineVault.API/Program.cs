using CineVault.API.Extensions;
using Microsoft.AspNetCore.Mvc;
[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

string environmentStr = builder.Environment.EnvironmentName;
Console.WriteLine(environmentStr);

builder.Services.AddCineVaultDbContext(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapControllers();

await app.RunAsync();