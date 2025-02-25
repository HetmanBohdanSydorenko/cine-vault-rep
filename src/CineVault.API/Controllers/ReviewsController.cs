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
public sealed class ReviewsController : ControllerBase
{
    private readonly CineVaultDbContext dbContext;
    private readonly ILogger<ReviewsController> logger;

    public ReviewsController(CineVaultDbContext dbContext, ILogger<ReviewsController> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    [HttpGet]
    [MapToApiVersion(1)]
    public async Task<ActionResult<List<ReviewResponse>>> GetReviewsVer1()
    {
        this.logger.LogInformation("Called GetReviewsVer1");
        var reviews = await this.dbContext.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .Select(r => new ReviewResponse
            {
                Id = r.Id,
                MovieId = r.MovieId,
                MovieTitle = r.Movie!.Title,
                UserId = r.UserId,
                Username = r.User!.Username,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
        return this.Ok(reviews);
    }

    [HttpOptions]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse<List<ReviewResponse>>>> GetReviewsVer2(
        ApiRequest request)
    {
        this.logger.LogInformation("Called GetReviewsVer2");
        var reviews = await this.dbContext.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .Select(r => new ReviewResponse
            {
                Id = r.Id,
                MovieId = r.MovieId,
                MovieTitle = r.Movie!.Title,
                UserId = r.UserId,
                Username = r.User!.Username,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
        return this.Ok(request.ToApiResponse(true, "Success", 200, reviews));
    }

    [HttpGet("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult<ReviewResponse>> GetReviewByIdVer1(int id)
    {
        this.logger.LogInformation("Called GetReviewByIdVer1 with id {ReviewId}", id);
        var review = await this.dbContext.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (review is null)
        {
            this.logger.LogWarning("Review with id {ReviewId} not found", id);
            return this.NotFound();
        }

        var response = new ReviewResponse
        {
            Id = review.Id,
            MovieId = review.MovieId,
            MovieTitle = review.Movie!.Title,
            UserId = review.UserId,
            Username = review.User!.Username,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
        return this.Ok(response);
    }

    [HttpOptions("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> GetReviewByIdVer2(
        ApiRequest request, int id)
    {
        this.logger.LogInformation("Called GetReviewByIdVer2 with id {ReviewId}", id);
        var review = await this.dbContext.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (review is null)
        {
            this.logger.LogWarning("Review with id {ReviewId} not found", id);
            return this.NotFound(request.ToApiResponse(false, "Failure", 404));
        }

        var response = new ReviewResponse
        {
            Id = review.Id,
            MovieId = review.MovieId,
            MovieTitle = review.Movie!.Title,
            UserId = review.UserId,
            Username = review.User!.Username,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
        return this.Ok(request.ToApiResponse(true, "Success", 200, response));
    }

    [HttpPost]
    [MapToApiVersion(1)]
    public async Task<ActionResult> CreateReviewVer1(ReviewRequest request)
    {
        this.logger.LogInformation("Called CreateReviewVer1 for MovieId {MovieId}, UserId {UserId}",
            request.MovieId, request.UserId);
        var review = new Review
        {
            MovieId = request.MovieId,
            UserId = request.UserId,
            Rating = request.Rating,
            Comment = request.Comment
        };
        this.dbContext.Reviews.Add(review);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("Review created for MovieId {MovieId}, UserId {UserId}",
            review.MovieId, review.UserId);
        return this.Created("", null);
    }

    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse>> CreateReviewVer2(ApiRequest<ReviewRequest> request)
    {
        this.logger.LogInformation("Called CreateReviewVer2 for MovieId {MovieId}, UserId {UserId}",
            request.Data.MovieId, request.Data.UserId);
        var review = new Review
        {
            MovieId = request.Data.MovieId,
            UserId = request.Data.UserId,
            Rating = request.Data.Rating,
            Comment = request.Data.Comment
        };
        this.dbContext.Reviews.Add(review);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("Review created for MovieId {MovieId}, UserId {UserId}",
            review.MovieId, review.UserId);
        return this.Ok(request.ToApiResponse(true, "Success", 201));
    }

    [HttpPut("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> UpdateReviewVer1(int id, ReviewRequest request)
    {
        this.logger.LogInformation("Called UpdateReviewVer1 for id {ReviewId}", id);
        var review = await this.dbContext.Reviews.FindAsync(id);
        if (review is null)
        {
            this.logger.LogWarning("UpdateReviewVer1: Review with id {ReviewId} not found", id);
            return this.NotFound();
        }

        review.MovieId = request.MovieId;
        review.UserId = request.UserId;
        review.Rating = request.Rating;
        review.Comment = request.Comment;
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("Review with id {ReviewId} updated successfully", id);
        return this.Ok();
    }

    [HttpPut("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse>> UpdateReviewVer2(int id,
        ApiRequest<ReviewRequest> request)
    {
        this.logger.LogInformation("Called UpdateReviewVer2 for id {ReviewId}", id);
        var review = await this.dbContext.Reviews.FindAsync(id);
        if (review is null)
        {
            this.logger.LogWarning("UpdateReviewVer2: Review with id {ReviewId} not found", id);
            return this.NotFound(request.ToApiResponse(false, "Failure", 404));
        }

        review.MovieId = request.Data.MovieId;
        review.UserId = request.Data.UserId;
        review.Rating = request.Data.Rating;
        review.Comment = request.Data.Comment;
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("Review with id {ReviewId} updated successfully", id);
        return this.Ok(request.ToApiResponse(true, "Success", 200));
    }

    [HttpDelete("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> DeleteReviewVer1(int id)
    {
        this.logger.LogInformation("Called DeleteReviewVer1 for id {ReviewId}", id);
        var review = await this.dbContext.Reviews.FindAsync(id);
        if (review is null)
        {
            this.logger.LogWarning("DeleteReviewVer1: Review with id {ReviewId} not found", id);
            return this.NotFound();
        }

        this.dbContext.Reviews.Remove(review);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("Review with id {ReviewId} deleted successfully", id);
        return this.Ok();
    }

    [HttpDelete("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse>> DeleteReviewVer2(int id, ApiRequest request)
    {
        this.logger.LogInformation("Called DeleteReviewVer2 for id {ReviewId}", id);
        var review = await this.dbContext.Reviews.FindAsync(id);
        if (review is null)
        {
            this.logger.LogWarning("DeleteReviewVer2: Review with id {ReviewId} not found", id);
            return this.NotFound(request.ToApiResponse(false, "Failure", 404));
        }

        this.dbContext.Reviews.Remove(review);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("Review with id {ReviewId} deleted successfully", id);
        return this.Ok(request.ToApiResponse(true, "Success", 200));
    }
}