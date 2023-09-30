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
        var movie = new Movie
        {
            Title = movieDTO.Title,
            Description = movieDTO.Description,
            ReleaseDate = movieDTO.ReleaseDate
        };

        await _movieRepository.CreateAsync(movie);

        return CreatedAtAction(nameof(Get), new{movieId = movie.Id}, movie);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieDTO>>> GetAll()
    {
        var movies = await _movieRepository.GetAllAsync();
        return Ok(movies.Select(m => new MovieDTO(
            m.Id,
            m.Title,
            m.Description,
            m.ReleaseDate
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

        return new MovieDTO(movie.Id, movie.Title, movie.Description, movie.ReleaseDate);
    }

    [HttpPut("{movieId}")]
    public async Task<ActionResult<Movie>> Update(int movieId, [FromBody] MovieCreateDTO movieDTO)
    {
        var movie = await _movieRepository.GetAsync(movieId);

        if(movie == null)
        {
            return NotFound();
        }

        movie.Title = movieDTO.Title;
        movie.Description = movieDTO.Description;
        movie.ReleaseDate = movieDTO.ReleaseDate;

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