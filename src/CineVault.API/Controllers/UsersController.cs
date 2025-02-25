using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineVault.API.Controllers;

[Route("api/[controller]/[action]")]
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
    public async Task<ActionResult<List<UserResponse>>> GetUsers()
    {
        this.logger.LogInformation("Called GetUsers");
        var users = await this.dbContext.Users
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            })
            .ToListAsync();

        return base.Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUserById(int id)
    {
        this.logger.LogInformation("Called GetUserById with id {UserId}", id);
        var user = await this.dbContext.Users.FindAsync(id);

        if (user is null)
        {
            this.logger.LogWarning("User with id {UserId} not found", id);
            return base.NotFound();
        }

        var response = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };

        return base.Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(UserRequest request)
    {
        this.logger.LogInformation("Called CreateUser with Username {Username}", request.Username);
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };

        this.dbContext.Users.Add(user);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("User created with Username {Username}", user.Username);
        return base.Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(int id, UserRequest request)
    {
        this.logger.LogInformation("Called UpdateUser for id {UserId}", id);
        var user = await this.dbContext.Users.FindAsync(id);

        if (user is null)
        {
            this.logger.LogWarning("UpdateUser: User with id {UserId} not found", id);
            return base.NotFound();
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.Password = request.Password;

        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("User with id {UserId} updated successfully", id);
        return base.Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        this.logger.LogInformation("Called DeleteUser for id {UserId}", id);
        var user = await this.dbContext.Users.FindAsync(id);

        if (user is null)
        {
            this.logger.LogWarning("DeleteUser: User with id {UserId} not found", id);
            return base.NotFound();
        }

        this.dbContext.Users.Remove(user);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("User with id {UserId} deleted successfully", id);
        return base.Ok();
    }
}