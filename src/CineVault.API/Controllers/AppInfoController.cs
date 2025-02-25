using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace CineVault.API.Controllers;

[Route("api/v{v:apiVersion}/[controller]/[action]")]
[ApiVersion(1)]
[ApiVersion(2)]
public class AppInfoController : ControllerBase
{
    [HttpGet]
    public string GetEnvironment(IWebHostEnvironment environment)
    {
        return environment.EnvironmentName;
    }

    [MapToApiVersion(1)]
    [HttpGet]
    public string GetCodeInFirstVersion()
    {
        string description = "The version of application: ";
        return description + 2021;
    }

    [MapToApiVersion(2)]
    [HttpGet]
    public string GetCodeInSecondVersion()
    {
        string description = "The version of this application will be ";
        return description + 2025;
    }
}