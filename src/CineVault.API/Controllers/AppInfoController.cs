using Microsoft.AspNetCore.Mvc;

namespace CineVault.API.Controllers;

[Route("api/[controller]/[action]")]
public class AppInfoController : ControllerBase
{
    [HttpGet]
    public string GetEnvironment(IWebHostEnvironment environment)
    {
        return environment.EnvironmentName;
    }
}