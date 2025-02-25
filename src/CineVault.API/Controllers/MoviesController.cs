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
public sealed class MoviesController : ControllerBase
{
    private readonly CineVaultDbContext dbContext;
    private readonly ILogger<MoviesController> logger;

    public MoviesController(CineVaultDbContext dbContext, ILogger<MoviesController> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    [HttpGet]
    [MapToApiVersion(1)]
    public async Task<ActionResult<List<MovieResponse>>> GetMoviesVer1()
    {
        this.logger.LogInformation("Called GetMoviesVer1");
        var movies = await this.dbContext.Movies
            .Include(m => m.Reviews)
            .Select(m => new MovieResponse
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ReleaseDate = m.ReleaseDate,
                Genre = m.Genre,
                Director = m.Director,
                AverageRating = m.Reviews.Count != 0
                    ? m.Reviews.Average(r => r.Rating)
                    : 0,
                ReviewCount = m.Reviews.Count
            })
            .ToListAsync();

        return this.Ok(movies);
    }

    [HttpOptions]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse<List<MovieResponse>>>> GetMoviesVer2(
        ApiRequest request)
    {
        this.logger.LogInformation("Called GetMoviesVer2");
        var movies = await this.dbContext.Movies
            .Include(m => m.Reviews)
            .Select(m => new MovieResponse
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ReleaseDate = m.ReleaseDate,
                Genre = m.Genre,
                Director = m.Director,
                AverageRating = m.Reviews.Count != 0
                    ? m.Reviews.Average(r => r.Rating)
                    : 0,
                ReviewCount = m.Reviews.Count
            })
            .ToListAsync();
        return this.Ok(request.ToApiResponse(true, "Success", 200, movies));
    }

    [HttpGet("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult<MovieResponse>> GetMovieByIdVer1(int id)
    {
        this.logger.LogInformation("Called GetMovieByIdVer1 with id {MovieId}", id);
        var movie = await this.dbContext.Movies
            .Include(m => m.Reviews)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie is null)
        {
            this.logger.LogWarning("Movie with id {MovieId} not found", id);
            return this.NotFound();
        }

        var response = new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Description,
            ReleaseDate = movie.ReleaseDate,
            Genre = movie.Genre,
            Director = movie.Director,
            AverageRating = movie.Reviews.Count != 0
                ? movie.Reviews.Average(r => r.Rating)
                : 0,
            ReviewCount = movie.Reviews.Count
        };

        return this.Ok(response);
    }
    
    [HttpOptions("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> GetMovieByIdVer2(ApiRequest request, int id)
    {
        this.logger.LogInformation("Called GetMovieByIdVer2 with id {MovieId}", id);
        var movie = await this.dbContext.Movies
            .Include(m => m.Reviews)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie is null)
        {
            this.logger.LogWarning("Movie with id {MovieId} not found", id);
            return this.NotFound(request.ToApiResponse(false, "Failure", 404));
        }

        var response = new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Description,
            ReleaseDate = movie.ReleaseDate,
            Genre = movie.Genre,
            Director = movie.Director,
            AverageRating = movie.Reviews.Count != 0
                ? movie.Reviews.Average(r => r.Rating)
                : 0,
            ReviewCount = movie.Reviews.Count
        };

        return this.Ok(request.ToApiResponse(true, "Success", 200, response));
    }

    [HttpPost]
    [MapToApiVersion(1)]
    public async Task<ActionResult> CreateMovieVer1(MovieRequest request)
    {
        this.logger.LogInformation("Called CreateMovieVer1 with Title {Title}", request.Title);
        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            ReleaseDate = request.ReleaseDate,
            Genre = request.Genre,
            Director = request.Director
        };

        await this.dbContext.Movies.AddAsync(movie);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("Movie created with Title {Title}", movie.Title);
        return this.Created();
    }
    
    [HttpPost]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse>> CreateMovieVer2(ApiRequest<MovieRequest> request)
    {
        this.logger.LogInformation("Called CreateMovieVer2 with Title {Title}", request.Data.Title);
        var movie = new Movie
        {
            Title = request.Data.Title,
            Description = request.Data.Description,
            ReleaseDate = request.Data.ReleaseDate,
            Genre = request.Data.Genre,
            Director = request.Data.Director
        };

        await this.dbContext.Movies.AddAsync(movie);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("Movie created with Title {Title}", movie.Title);
        return this.Ok(request.ToApiResponse(true, "Success", 201));
    }

    [HttpPut("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> UpdateMovieVer1(int id, MovieRequest request)
    {
        this.logger.LogInformation("Called UpdateMovieVer1 for id {MovieId}", id);
        var movie = await this.dbContext.Movies.FindAsync(id);

        if (movie is null)
        {
            this.logger.LogWarning("UpdateMovie: Movie with id {MovieId} not found", id);
            return this.NotFound();
        }

        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.ReleaseDate = request.ReleaseDate;
        movie.Genre = request.Genre;
        movie.Director = request.Director;

        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("Movie with id {MovieId} updated successfully", id);
        return this.Ok();
    }
    
    [HttpPut("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse>> UpdateMovieVer2(int id, ApiRequest<MovieRequest> request)
    {
        this.logger.LogInformation("Called UpdateMovieVer2 for id {MovieId}", id);
        var movie = await this.dbContext.Movies.FindAsync(id);

        if (movie is null)
        {
            this.logger.LogWarning("UpdateMovie: Movie with id {MovieId} not found", id);
            return this.NotFound(request.ToApiResponse(false, "Failure", 404));
        }

        movie.Title = request.Data.Title;
        movie.Description = request.Data.Description;
        movie.ReleaseDate = request.Data.ReleaseDate;
        movie.Genre = request.Data.Genre;
        movie.Director = request.Data.Director;

        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("Movie with id {MovieId} updated successfully", id);
        return this.Ok(request.ToApiResponse(true, "Success", 200));
    }

    [HttpDelete("{id}")]
    [MapToApiVersion(1)]
    public async Task<ActionResult> DeleteMovieVer1(int id)
    {
        this.logger.LogInformation("Called DeleteMovieVer1 for id {MovieId}", id);
        var movie = await this.dbContext.Movies.FindAsync(id);

        if (movie is null)
        {
            this.logger.LogWarning("DeleteMovie: Movie with id {MovieId} not found", id);
            return this.NotFound();
        }

        this.dbContext.Movies.Remove(movie);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("Movie with id {MovieId} deleted successfully", id);
        return this.Ok();
    }
    
    [HttpDelete("{id}")]
    [MapToApiVersion(2)]
    public async Task<ActionResult<ApiResponse>> DeleteMovieVer2(int id, ApiRequest request)
    {
        this.logger.LogInformation("Called DeleteMovieVer2 for id {MovieId}", id);
        var movie = await this.dbContext.Movies.FindAsync(id);

        if (movie is null)
        {
            this.logger.LogWarning("DeleteMovie: Movie with id {MovieId} not found", id);
            return this.NotFound(request.ToApiResponse(false, "Failure", 404));
        }

        this.dbContext.Movies.Remove(movie);
        await this.dbContext.SaveChangesAsync();
        this.logger.LogInformation("Movie with id {MovieId} deleted successfully", id);
        return this.Ok(request.ToApiResponse(true, "Success", 200));
    }
}