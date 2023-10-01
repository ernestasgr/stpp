using System.Text.RegularExpressions;
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

        return CreatedAtAction(nameof(Get), new{movieId = movie.Id}, movieDTO);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieDTO>>> GetAll()
    {
        var movies = await _movieRepository.GetAllAsync();
        return Ok(movies.Select(m => new MovieDTO(
            m.Id,
            m.Title,
            m.Description,
            m.ReleaseDate,
            m.Director
        )));
    }

    [HttpGet("{movieId}")]
    public async Task<ActionResult<MovieDTO>> Get(int movieId)
    {
        var movie = await _movieRepository.GetAsync(movieId);

        if(movie == null)
        {
            return NotFound();
        }

        return new MovieDTO(movie.Id, movie.Title, movie.Description, movie.ReleaseDate, movie.Director);
    }

    [HttpPut("{movieId}")]
    public async Task<ActionResult<MovieDTO>> Update(int movieId, [FromBody] MovieCreateDTO movieDTO)
    {
        var movie = await _movieRepository.GetAsync(movieId);

        if(movie == null)
        {
            return NotFound();
        }

        if(movieDTO.ReleaseDate.Kind != DateTimeKind.Utc)
        {
            return BadRequest("Release date must match the UTC format");
        }

        movie.Title = movieDTO.Title;
        movie.Description = movieDTO.Description;
        movie.ReleaseDate = movieDTO.ReleaseDate;
        movie.Director = movieDTO.Director;

        await _movieRepository.UpdateAsync(movie);

        return Ok(movieDTO);
    }

    [HttpDelete("{movieId}")]
    public async Task<ActionResult> Remove(int movieId)
    {
        var movie = await _movieRepository.GetAsync(movieId);

        if(movie == null)
        {
            return NotFound();
        }

        await _movieRepository.RemoveAsync(movie);

        return NoContent();
    }
}