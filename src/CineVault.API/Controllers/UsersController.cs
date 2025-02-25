using Asp.Versioning;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineVault.API.Controllers;

[Route("api/v{v:apiVersion}/[controller]/[action]")]
[ApiVersion(1)]
[ApiVersion(2)]
public class UsersController : ControllerBase
{
    private readonly CineVaultDbContext dbContext;
    private readonly ILogger<UsersController> logger;

    public UsersController(CineVaultDbContext dbContext, ILogger<UsersController> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    [HttpGet]
    [MapToApiVersion(1)]
    public async Task<ActionResult<List<UserResponse>>> GetUsersVer1()
    {
        this.logger.LogInformation("Called GetUsersVer1");
        var users = await this.dbContext.Users
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            })
            .ToListAsync();
        return this.Ok(users);
    }

    [HttpOptions]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse<List<UserResponse>>>> GetUsersVer2(ApiRequest request)
    {
        this.logger.LogInformation("Called GetUsersVer2");
        var users = await this.dbContext.Users
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            })
            .ToListAsync();
        return this.Ok(request.ToApiResponse(true, "Success", 200, users));
    }

    [HttpGet("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult<UserResponse>> GetUserByIdVer1(int id)
    {
        this.logger.LogInformation("Called GetUserByIdVer1 with id {UserId}", id);
        var user = await this.dbContext.Users.FindAsync(id);
        if (user is null)
        {
            this.logger.LogWarning("User with id {UserId} not found", id);
            return this.NotFound();
        }
        var response = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
        return this.Ok(response);
    }

    [HttpOptions("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetUserByIdVer2(ApiRequest request, int id)
    {
        this.logger.LogInformation("Called GetUserByIdVer2 with id {UserId}", id);
        var user = await this.dbContext.Users.FindAsync(id);
        if (user is null)
        {
            this.logger.LogWarning("User with id {UserId} not found", id);
            return this.NotFound(request.ToApiResponse(false, "Failure", 404));
        }
        var response = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
        return this.Ok(request.ToApiResponse(true, "Success", 200, response));
    }

    [HttpPost]
    [MapToApiVersion(1)]
    public async Task<ActionResult> CreateUserVer1(UserRequest request)
    {
        this.logger.LogInformation("Called CreateUserVer1 with Username {Username}", request.Username);
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };
        this.dbContext.Users.Add(user);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("User created with Username {Username}", user.Username);
        return this.Ok();
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse>> CreateUserVer2(ApiRequest<UserRequest> request)
    {
        this.logger.LogInformation("Called CreateUserVer2 with Username {Username}", request.Data.Username);
        var user = new User
        {
            Username = request.Data.Username,
            Email = request.Data.Email,
            Password = request.Data.Password
        };
        this.dbContext.Users.Add(user);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("User created with Username {Username}", user.Username);
        return this.Ok(request.ToApiResponse(true, "Success", 200, user.Username));
    }

    [HttpPut("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> UpdateUserVer1(int id, UserRequest request)
    {
        this.logger.LogInformation("Called UpdateUserVer1 for id {UserId}", id);
        var user = await this.dbContext.Users.FindAsync(id);
        if (user is null)
        {
            this.logger.LogWarning("UpdateUserVer1: User with id {UserId} not found", id);
            return this.NotFound();
        }
        user.Username = request.Username;
        user.Email = request.Email;
        user.Password = request.Password;
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("User with id {UserId} updated successfully", id);
        return this.Ok();
    }

    [HttpPut("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse>> UpdateUserVer2(int id, ApiRequest<UserRequest> request)
    {
        this.logger.LogInformation("Called UpdateUserVer2 for id {UserId}", id);
        var user = await this.dbContext.Users.FindAsync(id);
        if (user is null)
        {
            this.logger.LogWarning("UpdateUserVer2: User with id {UserId} not found", id);
            return this.NotFound(request.ToApiResponse(false, "Failure", 404));
        }
        user.Username = request.Data.Username;
        user.Email = request.Data.Email;
        user.Password = request.Data.Password;
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("User with id {UserId} updated successfully", id);
        return this.Ok(request.ToApiResponse(true, "Success", 200));
    }

    [HttpDelete("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> DeleteUserVer1(int id)
    {
        this.logger.LogInformation("Called DeleteUserVer1 for id {UserId}", id);
        var user = await this.dbContext.Users.FindAsync(id);
        if (user is null)
        {
            this.logger.LogWarning("DeleteUserVer1: User with id {UserId} not found", id);
            return this.NotFound();
        }

        this.dbContext.Users.Remove(user);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("User with id {UserId} deleted successfully", id);
        return this.Ok();
    }

    [HttpDelete("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse>> DeleteUserVer2(int id, ApiRequest request)
    {
        this.logger.LogInformation("Called DeleteUserVer2 for id {UserId}", id);
        var user = await this.dbContext.Users.FindAsync(id);
        if (user is null)
        {
            this.logger.LogWarning("DeleteUserVer2: User with id {UserId} not found", id);
            return this.NotFound(request.ToApiResponse(false, "Failure", 404));
        }

        this.dbContext.Users.Remove(user);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("User with id {UserId} deleted successfully", id);
        return this.Ok(request.ToApiResponse(true, "Success", 200));
    }
}