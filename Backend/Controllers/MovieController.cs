using System.Text.Json;
using backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/v1/movies")]
public class MovieController : ControllerBase
{
    private readonly IRepository<Movie> _movieRepository;
    public MovieController(IRepository<Movie> movieRepository)
    {
        _movieRepository = movieRepository;
    }

    [HttpPost]
    public async Task<ActionResult<MovieDTO>> Create([FromBody] MovieCreateDTO movieDTO)
    {
        if(movieDTO.ReleaseDate.Kind != DateTimeKind.Utc)
        {
            //400
            return BadRequest("Release date must match the UTC format");
        }

        var movie = new Movie
        {
            Title = movieDTO.Title,
            Description = movieDTO.Description,
            ReleaseDate = movieDTO.ReleaseDate,
            Director = movieDTO.Director
        };

        await _movieRepository.CreateAsync(movie);

        //201
        return CreatedAtAction(nameof(Get), new{movieId = movie.Id},
            new MovieDTO(movie.Id, movie.Title, movie.Description, movie.ReleaseDate, movie.Director));
    }

    [HttpGet(Name = "GetMany")]
    public async Task<ActionResult<IEnumerable<MovieDTO>>> GetMany([FromQuery] SearchParameters parameters)
    {
        var movies = await _movieRepository.GetManyAsync(parameters: parameters);

        var previousPageLink = movies.HasPrevious ?
            CreateMoviesResourceUri(parameters, ResourceUriType.PreviousPage) : null;

        var nextPageLink = movies.HasNext ?
            CreateMoviesResourceUri(parameters, ResourceUriType.NextPage) : null;

        var paginationMetadata = new
        {
            totalCount = movies.TotalCount,
            pageSize = movies.PageSize,
            currentPage = movies.CurrentPage,
            totalPages = movies.TotalPages,
            previousPageLink,
            nextPageLink
        };

        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

        //200
        return Ok(movies.Select(m => new MovieDTO(
            m.Id,
            m.Title,
            m.Description,
            m.ReleaseDate,
            m.Director
        )));
    }

    [HttpGet("{movieId}", Name="Get")]
    public async Task<ActionResult<MovieDTO>> Get(int movieId)
    {
        var movie = await _movieRepository.GetAsync(movieId);

        if(movie == null)
        {
            //404
            return NotFound();
        }

        //200
        return new MovieDTO(movie.Id, movie.Title, movie.Description, movie.ReleaseDate, movie.Director);
    }

    [HttpPut("{movieId}")]
    public async Task<ActionResult<MovieDTO>> Update(int movieId, [FromBody] MovieCreateDTO movieDTO)
    {
        var movie = await _movieRepository.GetAsync(movieId);

        if(movie == null)
        {
            //404
            return NotFound();
        }

        if(movieDTO.ReleaseDate.Kind != DateTimeKind.Utc)
        {
            //400
            return BadRequest("Release date must match the UTC format");
        }

        movie.Title = movieDTO.Title;
        movie.Description = movieDTO.Description;
        movie.ReleaseDate = movieDTO.ReleaseDate;
        movie.Director = movieDTO.Director;

        await _movieRepository.UpdateAsync(movie);

        //200
        return Ok(new MovieDTO(movie.Id, movie.Title, movie.Description, movie.ReleaseDate, movie.Director));
    }

    [HttpDelete("{movieId}", Name="Remove")]
    public async Task<ActionResult> Remove(int movieId)
    {
        var movie = await _movieRepository.GetAsync(movieId);

        if(movie == null)
        {
            //404
            return NotFound();
        }

        await _movieRepository.RemoveAsync(movie);

        //204
        return NoContent();
    }

    private string? CreateMoviesResourceUri(
        SearchParameters movieSearchParametersDto,
        ResourceUriType type)
    {
        var result = type switch
        {
            ResourceUriType.PreviousPage => Url.Link("GetMany",
                new
                {
                    pageNumber = movieSearchParametersDto.PageNumber - 1,
                    pageSize = movieSearchParametersDto.PageSize,
                }),
            ResourceUriType.NextPage => Url.Link("GetMany",
                new
                {
                    pageNumber = movieSearchParametersDto.PageNumber + 1,
                    pageSize = movieSearchParametersDto.PageSize,
                }),
            _ => Url.Link("GetMany",
                new
                {
                    pageNumber = movieSearchParametersDto.PageNumber,
                    pageSize = movieSearchParametersDto.PageSize,
                })
        };
        return result;
    }
}